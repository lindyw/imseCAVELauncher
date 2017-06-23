/*
    -----------------------
    UDP-Send2
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
 
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
 
    // nc -lu 127.0.0.1 8050
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class UDPSend : MonoBehaviour
{
	public InputField ip_InputField;
	public InputField port_InputField;

	public InputField cmd_InputField;

    private static int localPort;

    // prefs
    public string IP;  // define in init

    public int port;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage = "";


    // call it from shell (as program)
    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.init();

        // as server sending endless
        sendObj.sendEndless(" endless infos \n");
    }

    // OnGUI
    void OnGUI()
    {
//        Rect rectObj = new Rect(40, 380, 200, 400);
//        GUIStyle style = new GUIStyle();
//        style.alignment = TextAnchor.UpperLeft;
//		GUI.Box(rectObj, "# UDPSend-Data\n"+IP+" "+ port + " #\n"
//                + "shell> nc -lu "+IP+"  " + port + " \n"
//                , style);
//
//        // ------------------------
//        // send it
//        // ------------------------
//        strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
//        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
//        {
//            sendString(strMessage + "\n");
//        }
    }

    // init
    public void init()
    {
        // Define endpoint from which the messages are sent.
        print("UDPSend.init()");

		// define IP Address
		if (ip_InputField.text != "") {
			IP = ip_InputField.text;
		} else {
			IP = "192.168.0.191";
		}

		// define UDP port
		if (port_InputField.text != "") {
			port = int.Parse(port_InputField.text);
		} else {
			port = 1025;
		}

        // ----------------------------
        // Send
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
		print (string.Format("Sending to {0} : {1}",IP, port ));
		print (string.Format ("Testing: nc -lu {0} : {1}", IP, port));

    }
		

    // sendData
    public void sendString()
    {
		string message = cmd_InputField.text; 
        try
        {   
			byte[] prefix = StringToByteArray("3a");
			byte[] suffix = StringToByteArray("0D");
            // Encode data with the UTF8 encoding to binary format .
			byte[] input = Encoding.UTF8.GetBytes(message);
			byte[] data = new byte[prefix.Length + input.Length + suffix.Length];
			System.Buffer.BlockCopy(prefix, 0, data, 0, prefix.Length);
			System.Buffer.BlockCopy(input, 0, data, prefix.Length, input.Length);
			System.Buffer.BlockCopy(suffix, 0, data, prefix.Length+input.Length, suffix.Length);
            string encode1 = ByteArrayToString(data);
			print(string.Format("Sending Command: {0} to {1}:{2}", encode1, IP, port));
            // Send the message to the remote client .
            client.Send(data, data.Length, remoteEndPoint);
            //}

			print(string.Format("Sent Command: {0} to {1}:{2}", message, IP, port));

        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

	public static byte[] StringToByteArray(String hex)
	{
		int NumberChars = hex.Length;
		byte[] bytes = new byte[NumberChars / 2];
		for (int i = 0; i < NumberChars; i += 2)
			bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		return bytes;
	}
	
    public static string ByteArrayToString(byte[] val)
    {
     string b = "";
     int len = val.Length;
     for(int i = 0; i < len; i++) {
        if(i != 0) {
           b += ",";
        }            
        b += val[i].ToString();
     }
     return b;
   }



    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString();


        }
        while (true);

    }



}