using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Compression;
using System.IO;
using System;

public class TestZip : MonoBehaviour {
    public string startPath;
    public string zipDir;
    public string[] filesToCompress;

    // Start is called before the first frame update
    void Start() {
    }

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
}
