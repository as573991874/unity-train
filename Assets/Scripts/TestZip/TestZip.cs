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
}
