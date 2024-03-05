using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestLoad : MonoBehaviour
{
    public Image image;
    public string path;
    public string localPath;
    public string vedioRemotePath;
    public string vedioPath;

    private Texture2D texture;
    private Sprite sprite;
    private byte[] vedioDatas;

    private int cacheIndex = -1;
    private int cacheTimes;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        cacheIndex = -1;
    }

    // ONGUI
    void OnGUI()
    {
        int h = 50;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Load PNG"))
        {
            Debug.Log("Load PNG");
            // LoadLocalPNG();
            StartCoroutine(LoadPNG());
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Load Local PNG"))
        {
            Debug.Log("Load Local PNG");
            LoadLocalPNG();
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Load Video"))
        {
            Debug.Log("Load Video");
            StartCoroutine(LoadVedio());
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Load Video2"))
        {
            Debug.Log("Load Video2");
            LoadNext(null, 0);
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Load Save Video"))
        {
            Debug.Log("Load And Save Video");
            LoadAndSaveNext(0);
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Destroy Video"))
        {
            Debug.Log("Destroy Video");
            vedioDatas = null;
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Save Video"))
        {
            Debug.Log("Save Video");
            SaveVedio();
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "DS"))
        {
            Debug.Log("Destroy Sprite");
            if (sprite)
            {
                Destroy(sprite);
                sprite = null;
            }
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "DT"))
        {
            Debug.Log("Destroy Texture");
            if (texture)
            {
                Destroy(texture);
                texture = null;
            }
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "GC"))
        {
            Debug.Log("SystemGC");
            System.GC.Collect();
        }

        h += 70;
        if (GUI.Button(new Rect((Screen.width - 100) / 2, h, 150, 60), "Unload"))
        {
            Debug.Log("UnloadUnusedAssets");
            Resources.UnloadUnusedAssets();
        }
    }

    IEnumerator LoadPNG()
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(path))
        {
            yield return www.SendWebRequest();
            if (www.isDone && !www.isNetworkError)
            {
                texture = DownloadHandlerTexture.GetContent(www);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                image.sprite = sprite;
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }

    void LoadLocalPNG()
    {
        byte[] fileData = System.IO.File.ReadAllBytes(localPath);
        texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        image.sprite = sprite;

        // byte[] binary = Array.Empty<byte>();
        // var fileStream = new FileStream(localPath, FileMode.Open, FileAccess.Read);
        // fileStream.Seek(0, SeekOrigin.Begin);
        // binary = new byte[fileStream.Length];
        // fileStream.Read(binary, 0, (int)fileStream.Length);
        // fileStream.Close();
    }

    IEnumerator LoadVedio()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(vedioRemotePath + 0 + ".mp4"))
        {
            yield return www.SendWebRequest();
            if (www.isDone && !www.isNetworkError)
            {
                // vedioDatas = www.downloadHandler.data;
                // Debug.Log(vedioDatas.Length);
                Debug.Log(www.downloadHandler.data);
                Debug.Log(www.downloadHandler.data.Length);
            }
            else
            {
                Debug.Log(www.error);
            }
        }
    }

    async void LoadVedio2(Action<byte[], int> callback, int index)
    {
        var bytes = new byte[0];
        await Task.Run(() =>
        {
            HttpWebRequest request = WebRequest.Create(vedioRemotePath + 0 + ".mp4") as HttpWebRequest;
            request.Method = "GET";
            request.Date = DateTime.Now;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream resStream = response.GetResponseStream())
            {
                BinaryReader br = new BinaryReader(resStream);
                bytes = br.ReadBytes((int)response.ContentLength);
            }
            callback(bytes, ++index);
            response.Close();
        });
    }

    async void LoadAndSaveVedio(Action<int> callback, int index)
    {
        await Task.Run(() =>
        {
            HttpWebRequest request = WebRequest.Create(vedioRemotePath + 0 + ".mp4") as HttpWebRequest;
            request.Method = "GET";
            request.Date = DateTime.Now;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            int bufferSize = 4096; // 设置缓冲区大小，根据需要调整
            string path = vedioPath + "-" + index;//Time.frameCount;
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            using (Stream resStream = response.GetResponseStream())
            {
                BinaryReader br = new BinaryReader(resStream);
                BinaryWriter bw = new BinaryWriter(fileStream);
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                do
                {
                    bytesRead = br.Read(buffer, 0, bufferSize);
                    bw.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }
            callback(++index);
        });
    }

    void LoadAndSaveNext(int index)
    {
        if (index >= 13)
        {
            index = 0;
        }
        Debug.Log("LoadAndSaveNext: " + index);
        LoadAndSaveVedio(LoadAndSaveNext, index);
    }

    void LoadNext(byte[] bytes, int index)
    {
        if (cacheIndex == -1)
        {
            cacheTimes = 5;
            cacheIndex = index;
            vedioDatas = bytes;
            Debug.Log("LoadNext: " + index);
        }
    }

    // update
    void Update()
    {
        if (cacheIndex != -1)
        {
            if (cacheTimes > 0)
            {
                cacheTimes--;
                return;
            }
            int index = cacheIndex;
            cacheIndex = -1;
            if (index >= 13)
            {
                index = 0;
            }
            Debug.Log("LoadNext: " + index);
            SaveVedio(index);
            vedioDatas = null;
            System.GC.Collect();
            LoadVedio2(LoadNext, index);
        }
    }

    void SaveVedio(int index = 0)
    {
        if (vedioDatas == null)
        {
            return;
        }

        string path = vedioPath + "-" + index;//Time.frameCount;
        // File.WriteAllBytes(path, vedioDatas);
        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            fileStream.Write(vedioDatas, 0, vedioDatas.Length);
        }
    }
}
