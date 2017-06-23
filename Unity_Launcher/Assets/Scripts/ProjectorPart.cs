using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorPart : MonoBehaviour, IProjectManagerScript {
    public Main main;
    public ProjectorManagerScript projectorManager;

    public static int numOfProjectors;

    public static string order = "";
    public static string mode = "FS";
    // Use this for initialization
    void Start () {
        ProjectorManagerScript.RegisterListener(this);
        //Invoke("UpdateBlackList", Time.deltaTime * 3);
    }
	
	// Update is called once per frame
	void Update () {
        if (order.Length > 0)
        {
            switch (order.ToUpper())
            {
                case "DETECT": Detect(); break;
                case "TURNON": TurnOn(); break;
                case "TURNOFF": TurnOff(); break;
                case "SET2D": Set2D(); break;
                case "SET3D": Set3D(); break;
				case "SET3D_DUALHEAD": Set3DDualHead (); break;
                case "QUIT": TurnOff(); Quit(); break;
                case "SHUTDOWN": TurnOff(); ShutDown(); break;
                case "UPDATE": UpdateProjecter(); break;
                case "CONNECT": ConnectProjector(); break;
				case "DISCONNECT": EndProjector (); break;
				case "HDMI": Set3DLowResCAVE (); break;
				case "DPORT": Set3DHighResCAVE (); break; 
            }
            order = "";
        }

        //if (Input.GetKeyDown(KeyCode.F5)) UpdateBlackList();
    }

    void UpdateBlackList()
    {
        if (ECFile.FileExists("../Resources/BlackList.txt"))
        {
            string[] data = ECFile.ReadLines("../Resources/BlackList.txt");
            projectorManager.RenewBlackList(data);
        }
    }

    private void OnApplicationQuit() {
        ProjectorManagerScript.UnregisterListener(this);
    }

	public void EndProjector()
	{
		projectorManager.CloseProjectorPorts ();
	}

    public void ConnectProjector()
    {
        projectorManager.ConnectProjectors();
    }

    public void UpdateProjecter()
    {
        projectorManager.UpdateProjectors();
    }

    public void Detect()
    {
        projectorManager.DetectProjectors();
    }

    public void TurnOn()
    {
        projectorManager.TurnOnProjectors();
    }

    public void TurnOff()
    {
        projectorManager.TerminateProjectors();
    }

    public void Set2D()
    {
        projectorManager.Turn2DProjectors();
    }

    public void Set3D()
    {
        projectorManager.Turn3DProjectors();
    }

	// Lindy Added
	public void Set3DDualHead()
	{
		projectorManager.Turn3DDualHeadProjectors ();
	}

	public void Set3DHighResCAVE()
	{
		projectorManager.HighResCAVE ();
	}

	public void Set3DLowResCAVE()
	{
		projectorManager.LowResCAVE ();
	}

    public void Quit()
    {
        Application.Quit();
    }
    public void ShutDown()
    {
        //projectorManager.ShutdownSystem();
        if (ECFile.FileExists("../Resources/ShutDown.bat")) System.Diagnostics.Process.Start("cmd", "/C start /min " + ECFile.Path("../Resources/ShutDown.bat"));
        Quit();
    }

    public void OnDetectProjectors(string[] portNames) {
        Debug.Log("onDetect: " + string.Join(" ", portNames));
        main._RenewProjectors(Math.Max(portNames.Length - main.projectorStoreCount, 0), portNames);
    }

    public void OnLoadDefaultProjectors(string[] portNames) {
        Debug.Log("[Not use] onLoad: " + string.Join(" ", portNames));
    }
}
