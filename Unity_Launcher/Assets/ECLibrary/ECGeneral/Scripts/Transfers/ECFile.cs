using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ECFile : MonoBehaviour
{
    public string directory = "";
    public string file = "file";
    public string extension = "txt";
    public string fileName
    {
        get { return file + "." + extension; }
    }
    public string[] data;
    public string[] value
    {
        get;
        set;
    }
    public string separator = " | ";
    public int dataLength = 0;

    void Awake()
    {
        InitializeData();
        if (directory == null) directory = "../FolderPath";
        if (file == null) file = "file";
        if (extension == null) extension = "txt";
    }
    public ECFile() : this("", "file", "txt") {}
    public ECFile(string path)
    {
        InitializeData();
        directory = DirectoryName(path);
        file = FileName(path);
        extension = Extension(path);
    }
    public ECFile(string directory, string file, string extension)
    {
        InitializeData();
        string path = Path(directory, file, extension);
        this.directory = DirectoryName(path);
        this.file = FileName(path);
        this.extension = Extension(path);
    }

    /* ============================== Path ============================== */
    public static string DirectoryName(string path)
    {
        if (path.Length <= 0) return ""; 
        int el = new FileInfo(path).Extension.Length;
        if (el <= 0) return new FileInfo(path).FullName;
        return new FileInfo(path).DirectoryName;
    }
    public static string FileName(string path)
    {
        if (path.Length <= 0) return "";
        string file = new FileInfo(path).Name;
        int el = new FileInfo(path).Extension.Length;
        if (el <= 0) return "";
        if (file.Length - el > 0) return file.Substring(0, file.Length - el);
        return file;
    }
    public static string Extension(string path)
    {
        if (path.Length <= 0) return "";
        string extension = new FileInfo(path).Extension;
        if (extension.Length > 1) return extension.Substring(1);
        return extension;
    }
    public static string Path(string path)
    {
        return new FileInfo(path).FullName;
    }
    public static string Path(string directory, string file, string extension)
    {
        return new FileInfo(directory + "/" + file + "." + extension).FullName;
    }

    public string Path()
    {
        return new FileInfo(directory + "/" + file + "." + extension).FullName;
    }

    /* ============================== Directory ============================== */
    public static bool DirectoryExists(string directory)
    {
        return Directory.Exists(directory);
    }
    public static void CreateDirectory(string directory)
    {
        Directory.CreateDirectory(directory);
    }
    public static void DeleteDirectory(string directory)
    {
        if (Directory.Exists(directory)) Directory.Delete(directory);
    }

    public bool DirectoryExists()
    {
        return Directory.Exists(directory);
    }
    public void CreateDirectory()
    {
        Directory.CreateDirectory(directory);
    }
    public void DeleteDirectory()
    {
        if (Directory.Exists(directory)) Directory.Delete(directory);
    }
    /* ============================== File ============================== */
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }
    public static void CreateFile(string path)
    {
        if (!Directory.Exists(path)) File.Create(path);
    }
    public static void DeleteFile(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }
    public static void ClearFile(string path)
    {
        if (File.Exists(path))
        {
            string[] data = { "" };
            File.WriteAllLines(path, data);
        }
    }

    public bool FileExists()
    {
        return File.Exists(Path());
    }
    public void CreateFile()
    {
        if (!Directory.Exists(Path())) File.Create(Path());
    }
    public void DeleteFile()
    {
        if (File.Exists(Path())) File.Delete(Path());
    }
    public void ClearFile()
    {
        ClearFile(Path());
    }

    public static string[] FileCounts(string directory, string file, string extension)
    {
        List<string> files = new List<string>();
        if (DirectoryExists(directory))
        {
            string path = Path(directory, file, extension);
            if (file.Length > 0) file = FileName(Path(directory, file, extension));
            if (extension.Length > 0) extension = Extension(Path(directory, file, extension));
            DirectoryInfo info = new DirectoryInfo(directory);
            FileInfo[] infos = info.GetFiles();
            for (int i = 0; i < infos.Length; i++)
            {
                if ((file == "" || infos[i].Name.Contains(file)) && (extension == "" || infos[i].Extension.Equals("." + extension)))
                {
                    files.Add(infos[i].FullName);
                }
            }
        }
        return files.ToArray();
    }
    public static string[] FileCounts(string directory, string extension)
    {
        return FileCounts(directory, "", extension);
    }
    public static string[] FileCounts(string directory)
    {
        return FileCounts(directory, "", "");
    }
    /* ============================== File Get Set ============================== */
    public static string[] ReadLines(string path)
    {
        string[] data = new string[0];
        if (File.Exists(path))
        {
            data = File.ReadAllLines(path, System.Text.Encoding.UTF8);
        }
        return data;
    }
    public static string ReadText(string path)
    {
        string data = "";
        if (File.Exists(path))
        {
            data = File.ReadAllText(path, System.Text.Encoding.UTF8);
        }
        return data;
    }
    public static byte[] ReadBytes(string path)
    {
        byte[] data = new byte[0];
        if (File.Exists(path))
        {
            data = File.ReadAllBytes(path);
        }
        return data;
    }
    public static void WriteLines(string path, string[] data)
    {
        if (File.Exists(path))
        {
            File.WriteAllLines(path, data);
        }
    }
    public static void WriteText(string path, string data)
    {
        if (File.Exists(path))
        {
            File.WriteAllText(path, data);
        }
    }
    public static void WriteBytes(string path, byte[] data)
    {
        if (File.Exists(path))
        {
            File.WriteAllBytes(path, data);
        }
    }

    public string[] ReadLines()
    {
        return ReadLines(Path());
    }
    public string ReadText()
    {
        return ReadText(Path());
    }
    public byte[] ReadBytes()
    {
        return ReadBytes(Path());
    }

    /* ============================== Data ============================== */
    public bool NullData()
    {
        return data == null || data.Length == 0 || data[0] == null;
    }
    public void InitializeData()
    {
        if (NullData())
        {
            data = new string[1];
            data[0] = "DataName";
        }
        value = new string[data.Length];
        for (int i = 0; i < data.Length; i++) value[i] = data[i];
        dataLength = 1;
    }
    public void GetData()
    {
        string[] tmp = ReadLines();
        if (tmp.Length > 0)
        {
            bool renew = false;
            if (NullData() || data.Length < tmp.Length && data.Length <= 1 && value[0].Equals("DataName"))
            {
                dataLength = tmp.Length;
                data = new string[tmp.Length];
                value = new string[tmp.Length];
                for (byte i = 0; i < tmp.Length; i++)
                {
                    value[i] = tmp[i];
                    string[] strs = ECCommons.Separate(separator, tmp[i]);
                    data[i] = strs[0];
                }
                return;
            }
            if (tmp.Length >= value.Length)
            {
                dataLength = value.Length;
                for (byte i = 0; i < value.Length; i++)
                {
                    if (tmp[i].Length > data[i].Length)
                    {
                        value[i] = tmp[i];
                        string[] strs = ECCommons.Separate(separator, tmp[i]);
                        data[i] = strs[0];
                    }
                    else renew = true;
                }
            }
            if (tmp.Length < value.Length || renew) SetData();
        }
    }
    public void SetData()
    {
        WriteLines(Path(), value);
    }
    public void ResizeData()
    {
        string[] tmpD = data;
        string[] tmpV = value;
        data = new string[dataLength];
        value = new string[dataLength];
        for (int i = 0; i < data.Length && i < tmpD.Length && i < tmpV.Length; i++)
        {
            data[i] = tmpD[i];
            value[i] = tmpV[i];
        }
    }

    /* ============================== Media ============================== */
    public static Texture2D LoadImage(string path)
    {
        byte[] bytes = ECFile.ReadBytes(path);
        Texture2D target = new Texture2D(2, 2, TextureFormat.BGRA32, false);
        target.LoadImage(bytes);
        return target;
    }
    public static void SavePNG(string path, Texture2D texture)
    {
        if (!DirectoryExists(path)) CreateDirectory(path);
        byte[] pngFile = texture.EncodeToPNG();
        File.WriteAllBytes(Path(path), pngFile);
    }
    public static void SaveJPG(string path, Texture2D texture)
    {
        if (!DirectoryExists(path)) CreateDirectory(path);
        byte[] jpgFile = texture.EncodeToJPG();
        File.WriteAllBytes(Path(path), jpgFile);
    }
    
    public static IEnumerator LoadWAV(AudioSource audio, string path){
        WWW www = new WWW("file:///" + Path(path));
        while(!www.isDone){
            yield return www;
        }
        audio.clip = www.audioClip;
    }

    //public static AudioClip LoadAudio(string path)
    //{
    //    path = DirectoryName(path) + "/" + FileName(path);
    //    AudioClip a = Resources.Load<AudioClip>(path);
    //    return a;
    //}

    //public static AudioClip LoadSound(string path)
    //{
    //    WAV wav = new WAV(path);
    //    Debug.Log(wav);
    //    AudioClip audioClip = AudioClip.Create("testSound", wav.SampleCount, 1, wav.Frequency, false, false);
    //    audioClip.SetData(wav.LeftChannel, 0);
    //    return audioClip;
    //}
}
