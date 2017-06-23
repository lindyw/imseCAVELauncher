using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class ECArduino : MonoBehaviour
{
    [System.Serializable]
    public enum Type
    {
        READ_WRITE = 0,
        READ_ONLY = 1,
        WRITE_ONLY = 2
    }

    public Type type = Type.READ_WRITE;
    SerialPort sp;
    public string portName;
    public int baud = 9600;
    public int timeout = 60;
    public int buffer = 60;
    public int encoding = 28591;
    public int readLength = 0;

    int index = -1;
    string storeName;

    public bool autoConnect = true;

    public ConnectionState state = ConnectionState.DISCONNECTED;
    public string receivedData = "";
    public string transferedData = "";
    public ECFile textFile;
    public KeyCode fileRefreshKey = KeyCode.F5;

    public bool isTransferring = false;

    int tmpBuffer;
    // Use this for initialization
    void Start()
    {
        GetFile();
    }
    public void GetFile()
    {
        if (textFile && textFile.FileExists())
        {
            textFile.GetData();
            for (int i = 0; i < textFile.value.Length; i++)
            {
                if (textFile.value[i].Length > 0)
                {
                    string[] data = ECCommons.Separate(textFile.separator, textFile.value[i]);
                    switch (i)
                    {
                        case 0: portName = data[1]; break;
                        case 1: timeout = int.Parse(data[1]); break;
                        case 2: buffer = int.Parse(data[1]); break;
                        case 3: readLength = int.Parse(data[1]); break;
                    }
                }
            }
        }
        storeName = portName;
        tmpBuffer = buffer;
        state = ConnectionState.DISCONNECTED;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(fileRefreshKey)) GetFile();
        switch (state)
        {
            case ConnectionState.DISCONNECTED:
                if (autoConnect) StartCoroutine(StartConnect());
                break;
            case ConnectionState.CONNECTED:
                if (ArduinoExisted())
                {
                    try
                    {
                        state = ConnectionState.ERROR;
                        if (!isTransferring && buffer <= 0 && type != Type.WRITE_ONLY)
                        {
                            string tmp = sp.ReadLine();
                            if (readLength <= 0 || readLength == tmp.Length) receivedData = tmp;
                        }
                        else if (isTransferring && type != Type.READ_ONLY)
                        {
                            sp.WriteLine(transferedData);
                            isTransferring = false;
                            buffer = tmpBuffer;
                        }
                        state = ConnectionState.CONNECTED;
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                    if (buffer > 0) buffer--;
                }
                break;
            case ConnectionState.ERROR:
                Disconnect();
                break;
        }
    }

    public bool ArduinoExisted()
    {
        if (index >= 0 && SerialPort.GetPortNames().Length > index && SerialPort.GetPortNames()[index].Equals(PortName())) return true;
        return ArduinoFound(PortName());
    }
    public bool ArduinoFound(string name)
    {
        index = -1;
        for (int i = 0; i < SerialPort.GetPortNames().Length; i++)
        {
            string sName = SerialPort.GetPortNames()[i];
            if (sName.Equals(name) && !sName.Equals("COM1") && !sName.Equals("COM2"))
            {
                index = i;
                return true;
            }
        }
        return false;
    }
    public string PortName()
    {
        if (portName.Length > 0) return portName;
        if (SerialPort.GetPortNames().Length > 0) return SerialPort.GetPortNames()[SerialPort.GetPortNames().Length - 1];
        return "";
    }
    public void Connect(string name, int baud, int timeout, int encoding)
    {
        if (ArduinoFound(name))
        {
            sp = new SerialPort(name, baud, Parity.None);
            sp.Encoding = System.Text.Encoding.GetEncoding(encoding);
            sp.ReadTimeout = timeout;
            sp.WriteTimeout = timeout;
            state = ConnectionState.ERROR;
            sp.Open();
            portName = sp.PortName;
            state = ConnectionState.CONNECTED;
            Debug.Log("Arduino Connect: " + portName);
        }
        else state = ConnectionState.DISCONNECTED;
    }
    public IEnumerator StartConnect()
    {
        buffer = tmpBuffer;
        if (ArduinoExisted())
        {
            state = ConnectionState.CONNECTING;
            while (buffer > 0)
            {
                buffer--;
                yield return 0;
            }
            Connect(PortName(), baud, timeout, encoding);
        }
        //buffer = timeout;
    }
    public void Disconnect()
    {
        state = ConnectionState.DISCONNECTED;
        if (sp != null && sp.IsOpen)
        {
            portName = storeName;
            sp.DiscardOutBuffer();
            sp.Dispose();
            sp.Close();
            index = -1;
            Debug.Log("Arduino DisConnect: " + portName);
        }
    }

    public void DataTransfer(string data)
    {
        transferedData = data;
        isTransferring = true;
    }
}
