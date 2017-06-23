using UnityEngine;
using System.Collections;
using System.IO;

public class ECFolderReader : MonoBehaviour
{
    public string directory = "../Resources/FolderReader";
    public string receiverFolder = "Receiver";
    public string transmitterFolder = "Transmitter";

    public int orderLength = 3;
    public bool autoGenFolder = false;
    public bool startUpClear = true;

    public string receivedData = "";
    public string transferedData = "";

    int current = 0;
    // Use this for initialization
    void Start()
    {
        if (orderLength > 0) orderLength++;
        receiverFolder = directory + "/" + receiverFolder;
        transmitterFolder = directory + "/" + transmitterFolder;

        if (startUpClear)
        {
            if (ReceiverFound()) Directory.Delete(receiverFolder, true);
            if (TransmitterFound()) Directory.Delete(transmitterFolder, true);
        }

        transferedData = ECCommons.FixLength(0, 0, orderLength);
    }

    // Update is called once per frame
    void Update()
    {
        if (autoGenFolder)
        {
            if (!ReceiverFound()) CreateReceiver();
            if (!TransmitterFound()) CreateTransmitter();
        }
        if (ReceiverFound()) GetData();
    }

    public void CreateReceiver()
    {
        ECFile.CreateDirectory(receiverFolder);
    }

    public void CreateTransmitter()
    {
        ECFile.CreateDirectory(transmitterFolder);
    }

    public void GetData()
    {
        DirectoryInfo[] d = new DirectoryInfo(receiverFolder).GetDirectories();
        if (d.Length <= 0) return;
        if (orderLength > 0) receivedData = d[d.Length - 1].Name.Substring(orderLength);
        else receivedData = d[d.Length - 1].Name;
        receivedData = receivedData.Replace("┌", "\\");
        receivedData = receivedData.Replace("┬", "/");
        receivedData = receivedData.Replace("┐", ":");
        receivedData = receivedData.Replace("├", "*");
        receivedData = receivedData.Replace("┼", "?");
        receivedData = receivedData.Replace("┤", "\"");
        receivedData = receivedData.Replace("└", "<");
        receivedData = receivedData.Replace("┴", ">");
        receivedData = receivedData.Replace("┘", "|");
        receivedData = receivedData.Replace("∫", " ");
        Directory.Delete(d[d.Length - 1].FullName, true);
    }

    public void DataTransfer(string data)
    {
        data = data.Replace("\\", "┌");
        data = data.Replace("/", "┬");
        data = data.Replace(":", "┐");
        data = data.Replace("*", "├");
        data = data.Replace("?", "┼");
        data = data.Replace("\"", "┤");
        data = data.Replace("<", "└");
        data = data.Replace(">", "┴");
        data = data.Replace("|", "┘");
        data = data.Replace(" ", "∫");
        transferedData = AddList(data);
        Directory.CreateDirectory(transmitterFolder + "/" + transferedData);
    }

    string AddList(string data)
    {
        if (orderLength > 0)
        {
            data = ECCommons.FixLength(current, 0, orderLength - 1) + "." + data;
            current++;
            if (current >= Mathf.Pow(10, orderLength - 1)) current = 0;
        }
        return data;
    }

    public bool ReceiverFound()
    {
        return ECFile.DirectoryExists(receiverFolder);
    }
    public bool TransmitterFound()
    {
        return ECFile.DirectoryExists(transmitterFolder);
    }
}
