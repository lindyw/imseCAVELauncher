/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;
 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
 
public class UDPReceive : MonoBehaviour {
   
    // receiving Thread
    Thread receiveThread;
 
    // udpclient object
    UdpClient client;
 
    // public
    public string IP = "127.0.0.1"; // default local
    public int port; // define > init

	public Text ip_InputField;
	public Text port_InputField;
 
    // infos
    public string lastReceivedUDPPacket="";
    public string allReceivedUDPPackets=""; // clean up this from time to time!
   
   
    // start from shell
    private static void Main()
    {
       UDPReceive receiveObj=new UDPReceive();
       receiveObj.init();
 
        string text="";
        do
        {
             text = Console.ReadLine();
        }
        while(!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {
       
        init();
    }
   
    // OnGUI
    void OnGUI()
    {
        Rect rectObj=new Rect(240,500,200,400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
		style.normal.textColor = Color.white; 
		GUI.Box(rectObj,"# UDPReceive\n "+ IP +port+" #\n"
                    + "shell> nc -u "+IP+" : "+port+" \n"
                    + "\nLast Packet: \n"+ lastReceivedUDPPacket
                    + "\n\nAll Messages: \n"+allReceivedUDPPackets
                ,style);
    }

	void OnDisable()
	{
		if (receiveThread != null) {
			receiveThread.Abort ();

			client.Close (); // Close the udp connection
		}
	}
       
    // init
	public void init()
    {
		// Endpoint definition, calling init().
        print("UDPSend.init()");
       
		// define IP Address
		if (ip_InputField.text != "") {
			IP = ip_InputField.text;
		} else {
			IP = "127.0.0.1";
		}

        // define UDP port
		if (port_InputField.text != "") {
			port = int.Parse(port_InputField.text);
		} else {
			port = 80;
		}
 
        // status
		print(string.Format("Sending to: {0} : {1}", IP, port));
		print(string.Format("Test-Sending to this Port: nc -u {0}  {1}", IP, port));
	
 
        // ----------------------------
        // Local endpoint (where messges are received).
        // Create a new thread to receive incoming message
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
 
    }
 
    // receive thread
    private  void ReceiveData()
    {
 
        client = new UdpClient(port);
        while (true)
        {
 
            try
            {
                // Receive Bytes.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
 
                // Bytes to text
                string text = Encoding.ASCII.GetString(data);
 
                // print text
                print(">> " + text);
               
                // latest UDPpacket
                lastReceivedUDPPacket=text;
               
                // ....All messages
                allReceivedUDPPackets=allReceivedUDPPackets+text;
               
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
   
    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets="";
        return lastReceivedUDPPacket;
    }
}
 