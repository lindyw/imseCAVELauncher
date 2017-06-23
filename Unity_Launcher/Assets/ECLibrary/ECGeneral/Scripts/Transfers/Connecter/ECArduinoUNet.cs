﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ECArduinoUNet : MonoBehaviour
{
    public int monitorSet = -1;
    public float monitorSetDelay = 60;

    public ECArduino arduinos;
    public ECNetwork networks;

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
        auto[1].isOn = networks.autoConnect;

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
        if (networks.autoConnect != auto[1].isOn) networks.autoConnect = auto[1].isOn;
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
        //Network
        if (!status[1].text.Equals(networks.state.ToString())) status[1].text = networks.state.ToString();
        switch (connectBtn[1].gameObject.activeSelf)
        {
            case true:
                if (auto[0].isOn || networks.state == ConnectionState.CONNECTING) connectBtn[1].gameObject.SetActive(false);
                break;
            default:
                if (!auto[0].isOn && networks.state != ConnectionState.CONNECTING) connectBtn[1].gameObject.SetActive(true);
                break;
        }
        if (!networks.HostFound()) btnText[1].text = "Search";
        else
            switch (networks.state)
            {
                case ConnectionState.CONNECTED: btnText[0].text = "Disonnect"; break;
                default: btnText[0].text = "Connect"; break;
            }
        //Data R/W
        if (quitByExisting == ECFile.FileExists(quitText))
        {
            ECFile.DeleteFile(quitText);
            arduinos.autoConnect = false;
            arduinos.Disconnect();
            networks.autoConnect = false;
            networks.Disconnect();
            Application.Quit();
        }
        else
        {
            if (networks.state == ConnectionState.CONNECTED)
            {
                if (!networks.transferedData.Equals(arduinos.receivedData))
                {
                    networks.DataTransfer(arduinos.receivedData);
                }
            }
            if (arduinos.state == ConnectionState.CONNECTED)
            {
                if (!arduinos.transferedData.Equals(networks.receivedData))
                {
                    arduinos.DataTransfer(networks.receivedData);
                }
            }
        }
        data = "Received: " + arduinos.receivedData + "\n" + "Transferred: " + networks.receivedData;
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

    public void ConnectUnity()
    {
        if (btnText[1].text.Equals("Search"))
        {
            networks.PollHost();
        }
        else
        {
            if (networks.state != ConnectionState.CONNECTED)
            {
                networks.PollHost();
                networks.Connect();
            }
            else if (networks.state == ConnectionState.CONNECTED)
            {
                networks.Disconnect();
            }
        }
    }
}
