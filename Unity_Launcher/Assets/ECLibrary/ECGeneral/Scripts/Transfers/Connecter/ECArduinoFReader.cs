using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ECArduinoFReader : MonoBehaviour
{
    public int monitorSet = -1;
    public float monitorSetDelay = 60;

    public ECArduino arduinos;
    public ECFolderReader fReaders;

    public string data;

    public Text[] status;
    public Button[] connectBtn;
    public Text[] btnText;
    public Toggle[] auto;
    public Text dataText;

    public string quitText = "../Resources/AppOpened.txt";
    public bool quitByExisting = true;
    // Use this for initialization
    void Start()
    {
        PlayerPrefs.DeleteAll();
        Screen.SetResolution(800, 600, false);
        auto[0].isOn = arduinos.autoConnect;
        auto[1].isOn = fReaders.autoGenFolder;

        Invoke("Ini", Time.deltaTime * monitorSetDelay);
    }

    void Ini()
    {
        if (monitorSet >= 0) ECCommons.SetMonitorIndex(monitorSet, false, 800, 600);
    }

    // Update is called once per frame
    void Update()
    {
        if (arduinos.autoConnect != auto[0].isOn) arduinos.autoConnect = auto[0].isOn;
        if (fReaders.autoGenFolder != auto[1].isOn) fReaders.autoGenFolder = auto[1].isOn;
        //Arduino
        if (!status[0].text.Equals(arduinos.state.ToString())) status[0].text = arduinos.state.ToString();
        switch (connectBtn[0].gameObject.activeSelf)
        {
            case true:
                if (auto[0].isOn || arduinos.state == ConnectionState.CONNECTING || !arduinos.ArduinoExisted()) connectBtn[0].gameObject.SetActive(false);
                break;
            default:
                if (!auto[0].isOn && arduinos.state != ConnectionState.CONNECTING && arduinos.ArduinoExisted()) connectBtn[0].gameObject.SetActive(true);
                break;
        }
        switch (arduinos.state)
        {
            case ConnectionState.CONNECTED: btnText[0].text = "Disonnect"; break;
            default: btnText[0].text = "Connect"; break;
        }
        //Folder
        switch (connectBtn[1].gameObject.activeSelf)
        {
            case true:
                if (auto[1].isOn || fReaders.ReceiverFound() && fReaders.TransmitterFound())
                {
                    connectBtn[1].gameObject.SetActive(false);
                    status[1].text = "EXIST";
                }
                break;
            default:
                if (!auto[1].isOn && (!fReaders.ReceiverFound() || !fReaders.TransmitterFound()))
                {
                    connectBtn[1].gameObject.SetActive(true);
                    status[1].text = "LOST";
                }
                break;
        }
        //Data R/W
        if (quitByExisting == ECFile.FileExists(quitText))
        {
            ECFile.DeleteFile(quitText);
            arduinos.autoConnect = false;
            arduinos.Disconnect();
            Application.Quit();
        }
        else
        {
            if (!fReaders.transferedData.Substring(fReaders.orderLength).Equals(arduinos.receivedData))
            {
                fReaders.DataTransfer(arduinos.receivedData);
            }
            if (arduinos.state == ConnectionState.CONNECTED)
            {
                if (!arduinos.transferedData.Equals(fReaders.receivedData))
                {
                    arduinos.DataTransfer(fReaders.receivedData);
                }
            }
        }
        data = "Received: " + arduinos.receivedData + "\n" + "Transferred: " + fReaders.receivedData;
        dataText.text = data;
    }

    public void ConnectArduino()
    {
        if (arduinos.state == ConnectionState.DISCONNECTED)
        {
            arduinos.StartCoroutine(arduinos.StartConnect());
        }
        else if (arduinos.state == ConnectionState.CONNECTED)
        {
            arduinos.Disconnect();
        }
    }

    public void CreateFolder()
    {
        if (!fReaders.ReceiverFound()) fReaders.CreateReceiver();
        if (!fReaders.TransmitterFound()) fReaders.CreateTransmitter();
    }
}
