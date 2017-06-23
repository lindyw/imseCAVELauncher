using UnityEngine;
using System.Collections;

public class ECNetwork : MonoBehaviour
{
    public enum Type
    {
        SERVER = 0, CLIENT = 1
    }
    public Type type = Type.CLIENT;
    public string connectName = "Project";
    public string appName = "App";
    public string comment = "None";
    public int totalUser = 2;
    public int port = 25002;

    public bool autoConnect = false;
    public bool useUnityServer = false;

    public string serverIP = "127.0.0.1";
    public int serverPort = 23466;
    public string facilitatorIP = "127.0.0.1";
    public int facilitatorPort = 50005;

    public bool isUpdated = true;

    public NetworkView view;
    public HostData[] hostData;
    public int userCount = 0;
    public NetworkPlayer player;
    public ConnectionState state = ConnectionState.DISCONNECTED;
    public string receivedData = "";
    public string transferedData = "";
    public ECFile textFile;
    public KeyCode fileRefreshKey = KeyCode.F5;
    // Use this for initialization
    void Start()
    {
        GetFile();
        view = ECCommons.SetComponent<NetworkView>(gameObject);
    }

    public void GetFile()
    {
        if (textFile && textFile.FileExists())
        {
            textFile.GetData();
            for (int i = 0; i < textFile.value.Length; i++)
            {
                string[] data = ECCommons.Separate(textFile.separator, textFile.value[i]);
                if (data.Length > 1)
                {
                    switch (i)
                    {
                        case 0: connectName = data[1]; break;
                        case 1: appName = data[1]; break;
                        case 2: useUnityServer = data[1].Contains("T"); break;
                        case 3: serverIP = data[1]; break;
                        case 4: serverPort = int.Parse(data[1]); break;
                        case 5: facilitatorIP = data[1]; break;
                        case 6: facilitatorPort = int.Parse(data[1]); break;
                    }
                }
            }
        }
        if (!useUnityServer)
        {
            MasterServer.ipAddress = serverIP;
            MasterServer.port = serverPort;
            Network.natFacilitatorIP = facilitatorIP;
            Network.natFacilitatorPort = facilitatorPort;
        }
        state = ConnectionState.DISCONNECTED;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(fileRefreshKey)) GetFile();
        if (autoConnect && state != ConnectionState.CONNECTED)
        {
            if (type == Type.SERVER)
            {
                OpenHost();
            }
            else
            {
                PollHost();
                Connect();
            }
        }
    }
    /* ------------------------------ Server ------------------------------ */
    /// <summary>
    /// Server Side: Inititalize server and register a host to connect.
    /// </summary>
    public void OpenHost()
    {
        Network.InitializeServer(totalUser, port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(connectName, appName, comment);
    }
    /// <summary>
    /// Server Side: Close and clear Host. 
    /// </summary>
    public void CloseHost()
    {
        Network.Disconnect();
        MasterServer.UnregisterHost();
        Disconnect();
    }

    /* ------------------------------ Client ------------------------------ */
    /// <summary>
    /// Search all possible hosts.
    /// </summary>
    public void PollHost()
    {
        MasterServer.RequestHostList(connectName);
        hostData = MasterServer.PollHostList();
        string name = "";
        foreach (HostData n in hostData) name += n.gameName + " ; ";
        Debug.Log("Total Host: " + hostData.Length + " | List: " + name);
    }
    /// <summary>
    /// Return true if there is any host.
    /// </summary>
    /// <returns></returns>
    public bool HostFound()
    {
        return (hostData != null && hostData.Length > 0);
    }
    /// <summary>
    /// Connect a host in host list.
    /// </summary>
    /// <param name="i"> The index of host </param>
    /// <returns></returns>
    public bool Connect(int i)
    {
        state = ConnectionState.CONNECTING;
        if (HostFound() && i < hostData.Length && hostData[i].connectedPlayers < hostData[i].playerLimit)
        {
            Debug.Log("Number of Players: " + hostData[i].connectedPlayers + "/" + (hostData[i].playerLimit - 1));
            Network.Connect(hostData[i]);
            state = ConnectionState.CONNECTED;
            //hostData = null;
            return true;
        }
        if (HostFound()) state = ConnectionState.FULL;
        else state = ConnectionState.DISCONNECTED;
        return false;
    }
    /// <summary>
    /// Connect to the first host.
    /// </summary>
    /// <returns></returns>
    public bool Connect()
    {
        return Connect(0);
    }

    /* ------------------------------ Mix ------------------------------ */
    /// <summary>
    /// Clear host and choose to disconnect network.
    /// </summary>
    /// <param name="disconnect"> The connection state will set to be DISCONNECTED if true </param>
    public void Disconnect(bool disconnect)
    {
        if (Network.isClient)
        {
            if (state != ConnectionState.DISCONNECTED) Network.CloseConnection(Network.connections[0], true);
            MasterServer.ClearHostList();
        }
        hostData = null;
        if (disconnect) state = ConnectionState.DISCONNECTED;
        player = new NetworkPlayer();
    }
    /// <summary>
    /// Clear host and disconnect network.
    /// </summary>
    public void Disconnect()
    {
        Disconnect(true);
    }
    /// <summary>
    /// Transfer all data by a combined string.
    /// </summary>
    /// <param name="data"> Dynamic length of any object type data </param>
    public void DataTransfer(string data)
    {
        transferedData = data;
        if (Network.connections.Length > 0)
        {
            view.RPC("DataReceive", RPCMode.OthersBuffered, transferedData);
            Debug.Log("Transfer: " + transferedData);
        }
    }
    /* ------------------------------ Unity Event ------------------------------ */
    void OnConnectedToServer()
    {
        state = ConnectionState.CONNECTED;
        Debug.Log("Player " + player + " is connected to server.");
    }
    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        state = ConnectionState.DISCONNECTED;
        if (Network.isServer)
            Debug.Log("Local server connection dstate");
        else
        {
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
            {
                Debug.Log("Successfully diconnected from the server");
            }
            Disconnect();
        }
    }
    void OnFailedToConnect(NetworkConnectionError error)
    {
        Disconnect();
        Debug.Log("Could not connect to server: " + error);
    }
    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        Debug.Log("New object instantiated by " + info.sender);
    }
    void OnPlayerConnected(NetworkPlayer player)
    {
        if (Network.connections.Length > 0)
        {
            view.RPC("PlayerIdentify", RPCMode.AllBuffered, player);
        }
        Debug.Log("Player " + player + " connected from " + player.ipAddress + ":" + player.port);
        userCount++;
    }
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        userCount--;
        Debug.Log("Clean up after player " + player + " is dstate");
    }
    /*
    public int currentHealth;
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
 
        int health = 0;
        if (stream.isWriting)
        {
            health = currentHealth;
            stream.Serialize(ref health);
        }
        else
        {
            stream.Serialize(ref health);
            currentHealth = health;
        }
    }
    */
    void OnServerInitialized()
    {
        state = ConnectionState.CONNECTED;
        Debug.Log("Server " + MasterServer.ipAddress + ":" + MasterServer.port + " initialized and ready");
    }
    void OnApplicationQuit()
    {
        Debug.Log("Quit");
        state = ConnectionState.DISCONNECTED;
        if (Network.isServer) CloseHost();
        else Disconnect();
        Application.Quit();
    }

    /* ------------------------------ RPC ------------------------------ */
    [RPC]
    void DataReceive(string transferedData)
    {
        //this.transferedData = transferedData;
        receivedData = transferedData;
        isUpdated = false;
    }
    [RPC]
    void PlayerIdentify(NetworkPlayer player)
    {
        if (type == Type.CLIENT && this.player == new NetworkPlayer())
        {
            this.player = player;
        }
    }
}
