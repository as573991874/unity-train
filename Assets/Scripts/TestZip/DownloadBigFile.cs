using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using UnityEngine;

class DownloadBigFile {
    // 下载地址
    private string url;

    // 本地存储路径
    private string filePath;

    // 本地解压目录
    private string extractPath;

    // 多线程下载
    private int numberOfThreads;

    // 多线程锁
    private object lockObject;

    // 文件总大小
    private long totalSize;
    public long TotalSize {
        set {
            lock (lockObject) {
                totalSize = value;
            }
        }
        get {
            lock (lockObject) {
                return totalSize;
            }
        }
    }

    // 已下载文件大小
    private long loadSize;
    public long LoadSize {
        set {
            lock (lockObject) {
                loadSize = value;
            }
        }
        get {
            lock (lockObject) {
                return loadSize;
            }
        }
    }

    // 是否解压中
    private bool inExtract;
    public bool InExtract {
        set {
            lock (lockObject) {
                inExtract = value;
            }
        }
        get {
            lock (lockObject) {
                return inExtract;
            }
        }
    }

    // 待解压文件总数量
    private int totalFileCount;
    public int TotalFileCount {
        set {
            lock (lockObject) {
                totalFileCount = value;
            }
        }
        get {
            lock (lockObject) {
                return totalFileCount;
            }
        }
    }

    // 已解压文件数量
    private int extractCount;
    public int ExtractCount {
        set {
            lock (lockObject) {
                extractCount = value;
            }
        }
        get {
            lock (lockObject) {
                return extractCount;
            }
        }
    }

    // 是否已下载并解压完成
    private bool isFinish;
    public bool IsFinish {
        set {
            lock (lockObject) {
                isFinish = value;
            }
        }
        get {
            lock (lockObject) {
                return isFinish;
            }
        }
    }

    // 下载完成回调
    private Action onFinish;

    public DownloadBigFile(string url, string filePath, string extractPath, int numberOfThreads, Action onFinish) {
        this.url = url;
        this.filePath = filePath;
        this.extractPath = extractPath;
        this.numberOfThreads = numberOfThreads;
        this.onFinish = onFinish;
        this.lockObject = new object();
        this.totalSize = 1;
        this.loadSize = 0;
        this.inExtract = false;
        this.totalFileCount = 1;
        this.extractCount = 0;
        this.isFinish = false;
    }

    public async Task Start() {
        // 先获取总大小
        this.TotalSize = await GetFileSize();
        Debug.Log($"GetFileSize {url} Finish: {this.TotalSize}");

        // 再多线程下载子文件
        long partSize = totalSize / numberOfThreads;
        Task[] tasks = new Task[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++) {
            long start = i * partSize;
            long end = (i == numberOfThreads - 1) ? totalSize - 1 : (start + partSize - 1);
            string partFilePath = $"{filePath}.part{i}";
            tasks[i] = Task.Run(() => DownloadFilePart(partFilePath, start, end));
        }
        await Task.WhenAll(tasks);
        Debug.Log($"Download {url} FilePart Finish");

        // 再合并文件
        MergeFileParts();
        Debug.Log($"MergeFile {filePath} Finish");

        // 再解压
        ExtractFile();
        Debug.Log($"ExtractFile {extractPath} Finish");

        // 完成
        this.IsFinish = true;
        Debug.Log($"Download And Extract Finish {url} -> {extractPath}");
        try {
            this.onFinish.Invoke();
        } catch (System.Exception e) {
            Debug.Log($"error {e.Message}");
        }
    }

    async Task<long> GetFileSize() {
        long size = 0;
        while (true) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync()) {
                    size = response.ContentLength;
                    break;
                }
            } catch (Exception e) {
                Debug.Log($"Error GetFileSize {url} : {e.Message}");
                await Task.Delay(2000);
            }
        }
        return size;
    }

    async Task DownloadFilePart(string filePath, long start, long end) {
        long existingLength = 0;
        if (File.Exists(filePath)) {
            existingLength = new FileInfo(filePath).Length;
        }

        if (existingLength >= end - start + 1) {
            Debug.Log($"Part {filePath} already downloaded.");
            return;
        }

        while (true) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AddRange(start + existingLength, end);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync()) {
                    using (Stream responseStream = response.GetResponseStream()) {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None)) {
                            byte[] buffer = new byte[1024 * 32];
                            int bytesRead;
                            while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                                // await fileStream.WriteAsync(buffer, 0, bytesRead);
                                fileStream.Write(buffer, 0, bytesRead);
                                this.LoadSize = this.LoadSize + bytesRead;
                                // Debug.Log("Task thread ID: " + Thread.CurrentThread.ManagedThreadId);
                            }
                        }
                    }
                }
                break;
            } catch (Exception e) {
                Debug.Log($"Error DownloadFilePart {filePath} : {e.Message}");
                await Task.Delay(2000);
            }
        }

        Debug.Log($"Download FilePart {filePath} Finish.");
    }

    void MergeFileParts() {
        using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create)) {
            for (int i = 0; i < numberOfThreads; i++) {
                string partFilePath = $"{filePath}.part{i}";
                using (FileStream partFileStream = new FileStream(partFilePath, FileMode.Open)) {
                    partFileStream.CopyTo(outputFileStream);
                }
                File.Delete(partFilePath);
            }
        }
    }

    void ExtractFile() {
        this.InExtract = true;
        try {
            using (ZipArchive archive = ZipFile.OpenRead(filePath)) {
                this.TotalFileCount = archive.Entries.Count;
                foreach (ZipArchiveEntry entry in archive.Entries) {
                    string destinationPath = Path.Combine(extractPath, entry.FullName);
                    // 如果目标文件存在
                    if (File.Exists(destinationPath)) {
                        this.extractCount++;
                        continue;
                    }

                    // 创建包含目标文件的目录（如果不存在）
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    // 异步解压文件
                    // await Task.Run(() => entry.ExtractToFile(destinationPath, true));
                    entry.ExtractToFile(destinationPath, true);
                    this.extractCount++;
                }
            }
            this.InExtract = false;
        } catch (Exception e) {
            Debug.Log($"Error ExtractFile {filePath} : {e.Message}");
        }
    }
}
