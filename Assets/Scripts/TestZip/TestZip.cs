using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.IO;
using System;
using System.Threading.Tasks;

public class TestZip : MonoBehaviour {
    [Header("压缩文件路径")]
    public string startPath;
    [Header("导出的压缩文件")]
    public string zipDir;
    [Header("压缩文件列表")]
    public string[] filesToCompress;

    [Header("需要下载的压缩文件")]
    public string url;

    [Header("下载后的本地文件")]
    public string filePath;

    [Header("本地解压目录")]
    public string extractPath;

    // 多线程下载
    [Header("多线程下载")]
    public int numberOfThreads;

    [Header("存在的压缩文件")]
    public string zipFile;

    [Header("解压后的目录")]
    public string zipExtractPath;

    [Header("读取的文件")]
    public string readFilePath;

    // Update is called once per frame
    void Update() {

    }

    void OnGUI() {
        // 屏幕中间按钮
        if (GUI.Button(new Rect((Screen.width - 100) / 2, 0, 100, 50), "ZipFile")) {
            ZipFileFunc();
        }

        if (GUI.Button(new Rect((Screen.width - 100) / 2, 60, 100, 50), "ZipFile2")) {
            ZipFileFunc2();
        }

        if (GUI.Button(new Rect((Screen.width - 100) / 2, 120, 100, 50), "DownloadSingle")) {
            DownloadSingle();
        }

        if (GUI.Button(new Rect((Screen.width - 100) / 2, 180, 100, 50), "DownloadMulti")) {
            DownloadMulti();
        }

        if (GUI.Button(new Rect((Screen.width - 100) / 2, 240, 100, 50), "ExtractSpeed")) {
            ExtractSpeed();
        }

        if (GUI.Button(new Rect((Screen.width - 100) / 2, 300, 100, 50), "ReadFile")) {
            ReadFileSpeed();
        }
    }

    void ZipFileFunc() {
        // 检查压缩包是否已经存在，如果存在则删除
        if (File.Exists(zipDir)) {
            File.Delete(zipDir);
        }

        // 创建压缩包
        ZipFile.CreateFromDirectory(startPath, zipDir);
        Debug.Log("压缩完成！");
    }

    void ZipFileFunc2() {
        // 检查压缩包是否已经存在，如果存在则删除
        if (File.Exists(zipDir)) {
            File.Delete(zipDir);
        }
        // 创建压缩包
        using (ZipArchive archive = ZipFile.Open(zipDir, ZipArchiveMode.Create)) {
            foreach (string file in filesToCompress) {
                // 压缩文件
                archive.CreateEntryFromFile(file, Path.GetFileName(file));
            }
        }
        Debug.Log("压缩完成！");
    }

    void DownloadSingle() {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var download = new DownloadBigFile(
            url, filePath, extractPath, 1,
            () => {
                stopwatch.Stop();
                TimeSpan timeTaken = stopwatch.Elapsed;
                Debug.Log($"Download finish : {timeTaken.ToString(@"m\:ss\.fff")}");
            }
        );
        Task.Run(download.Start);
    }

    void DownloadMulti() {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var download = new DownloadBigFile(
            url, filePath, extractPath, numberOfThreads,
            () => {
                stopwatch.Stop();
                TimeSpan timeTaken = stopwatch.Elapsed;
                Debug.Log($"Download finish : {timeTaken.ToString(@"m\:ss\.fff")}");
            }
        );
        Task.Run(download.Start);
    }

    void ExtractSpeed() {
        // 准备文件夹
        if (Directory.Exists(zipExtractPath)) {
            Directory.Delete(zipExtractPath, true);
        }
        Directory.CreateDirectory(zipExtractPath);

        // 按文件夹解压
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        ZipFile.ExtractToDirectory(zipFile, zipExtractPath);
        stopwatch.Stop();
        Debug.Log($"Extracting by folder took: {stopwatch.Elapsed.TotalSeconds} seconds");

        // 清理文件夹
        if (Directory.Exists(zipExtractPath)) {
            Directory.Delete(zipExtractPath, true);
        }
        Directory.CreateDirectory(zipExtractPath);

        // 按文件解压
        stopwatch.Reset();
        stopwatch.Start();
        using (ZipArchive archive = ZipFile.OpenRead(zipFile)) {
            foreach (ZipArchiveEntry entry in archive.Entries) {
                string destinationPath = Path.GetFullPath(Path.Combine(zipExtractPath, entry.FullName));
                if (entry.Name == "") {
                    // 创建目录
                    Directory.CreateDirectory(destinationPath);
                } else {
                    // 创建目录（如果不存在）
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    entry.ExtractToFile(destinationPath, true);
                }
            }
        }
        stopwatch.Stop();
        Debug.Log($"Extracting by file took: {stopwatch.Elapsed.TotalSeconds} seconds");
    }

    void ReadFileSpeed() {
        // 从 zip 文件中读取文件流
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        ZipArchive archive = ZipFile.OpenRead(zipFile);
        ZipArchiveEntry entry = archive.GetEntry(readFilePath);
        if (entry != null) {
            using (Stream zipStream = entry.Open()) {
                // 这里可以对 zipStream 进行处理，例如读取内容
                using (MemoryStream ms = new MemoryStream()) {
                    zipStream.CopyTo(ms);
                }
            }
        }
        stopwatch.Stop();
        Debug.Log($"Reading from zip file took: {stopwatch.Elapsed.TotalMilliseconds} ms");

        // 从缓存的 archive 中读取文件流
        stopwatch.Reset();
        stopwatch.Start();
        entry = archive.GetEntry(readFilePath);
        if (entry != null) {
            using (Stream zipStream = entry.Open()) {
                // 这里可以对 zipStream 进行处理，例如读取内容
                using (MemoryStream ms = new MemoryStream()) {
                    zipStream.CopyTo(ms);
                }
            }
        }
        stopwatch.Stop();
        Debug.Log($"Reading from zip archive took: {stopwatch.Elapsed.TotalMilliseconds} ms");
        archive.Dispose();

        // 直接从本地文件读取文件流
        stopwatch.Reset();
        stopwatch.Start();
        using (FileStream localStream = File.OpenRead(Path.Combine(zipExtractPath, readFilePath))) {
            // 这里可以对 localStream 进行处理，例如读取内容
            using (MemoryStream ms = new MemoryStream()) {
                localStream.CopyTo(ms);
            }
        }
        stopwatch.Stop();
        Debug.Log($"Reading from local file took: {stopwatch.Elapsed.TotalMilliseconds} ms");
    }
}
