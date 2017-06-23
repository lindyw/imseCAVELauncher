using UnityEngine;
using UnityEngine.UI;
using SystemD = System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text.RegularExpressions;

public class ProjectorManagerScript : MonoBehaviour {

    public GameObject commandsBlock;
    public Button commandTemplate;
    public List<string> projectorCommands;
    public ProjectorScript projectorObject;
    public CustomProjectorScript customProjectObject;
    public string portNameDetectPattern;
    public InputField logText;
    public bool willAddProjector = false;

    public const string PREF_DEFAULT_PORT_NAMES_KEY = "_pref_default_port_names";
    public const string PREF_DEFAULT_EXCLUDED_PORT_NAMES_KEY = "_pref_default_excluded_port_names";
    public const string PROJECTOR_NOT_CONNECTED_STRING = "Not connected";
    public const string PROJECTOR_NOT_DETECTED_STRING = "Cannot detect this port. Please check connection and detect again.";
    public const string PROJECTOR_NOT_SAVED_STRING = "Cannot load this port. Please detect or input it.";
    public static string[] PROJECTOR_ERROR_STRINGS = new string[] { PROJECTOR_NOT_DETECTED_STRING , PROJECTOR_NOT_SAVED_STRING };
    public static string[] availablePortNames;

    private List<string> projectorPortNames;
    private List<string> projectorPortNamesExcluded;
    private List<IProjectorScript> projectors;
    private Regex portNameDetectRegex;
    private bool willBeShutdown = true;
    private Coroutine shutdownCoroutine;
    private static List<IProjectManagerScript> listeners;
    private static List<Action<IProjectManagerScript>> listenerCallbacks;

    // init once only
    private void Awake() {
        if (projectors == null) {
            projectors = new List<IProjectorScript>();
        }

        InitListeners();
    }

    // Use this for initialization
    void Start () {
        /*
         * Enable the function call below to clean the PlayerPrefs
         */
        //PlayerPrefs.DeleteKey(PREF_DEFAULT_PORT_NAMES_KEY);
        //PlayerPrefs.DeleteKey(PREF_DEFAULT_EXCLUDED_PORT_NAMES_KEY);

        //InitListeners();

        ProjectorManagerScript.availablePortNames = SerialPort.GetPortNames();
        string portNamesStr = "Found ports: " + string.Join(", ", ProjectorManagerScript.availablePortNames);
        Debug.Log(portNamesStr);

        portNameDetectRegex = new Regex(portNameDetectPattern == "" ? ".*" : portNameDetectPattern);

        // get default port name
        string defaultPortNamesStr = PlayerPrefs.GetString(PREF_DEFAULT_PORT_NAMES_KEY);
        Debug.Log("default ports: " + defaultPortNamesStr);
        string[] defaultPortNames = Array.FindAll(defaultPortNamesStr.Split(','), s => s != "");
        projectorPortNamesExcluded = new List<string>();
        projectorPortNames = new List<string>(defaultPortNames);
        LoopListeners(i => i.OnDetectProjectors(projectorPortNames.ToArray()));

        Debug.Log("Create Projector ports");
        InitProjectors();
        
        // get default excluded port name
        //string defaultExcludedPortNameStr = PlayerPrefs.GetString(PREF_DEFAULT_EXCLUDED_PORT_NAMES_KEY);
        //Debug.Log("Blacklist: " + defaultExcludedPortNameStr);
        //string[] defaultExcludedPortName = Array.FindAll(defaultExcludedPortNameStr.Split(','), str => str != "");
        //projectorPortNamesExcluded = new List<string>(defaultExcludedPortName);

        if (commandsBlock != null && commandTemplate != null && projectorCommands != null) {
            Debug.Log("Create command blocks");
            InitCommandBlock();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable () {
        if (logText != null) {
            Application.logMessageReceived += LogHandler;
        }
    }

    void OnDisable() {
        if (logText != null) {
            Application.logMessageReceived -= LogHandler;
        }
    }

    void OnApplicationQuit() {
        Debug.Log("Quit");

        // save excluded port names
        //PlayerPrefs.SetString(PREF_DEFAULT_EXCLUDED_PORT_NAMES_KEY, String.Join(",", projectorPortNamesExcluded.ToArray()));

        CloseProjectorPorts();
    }

    private void InitCommandBlock () {
        foreach (string cmd in projectorCommands) {
            string[] cmdElements = cmd.Split(';');
            string btnMethod = cmdElements[1];
            string btnName = cmdElements[0];
            bool mustBeWorking = cmdElements.Length >= 3 ? (cmdElements[2] == "1") : true;
            //Debug.Log(btnName + " : " + btnMethod);

            Button b = Instantiate<Button>(commandTemplate);
            b.transform.SetParent(commandsBlock.transform);
            b.transform.localScale = Vector3.one;
            b.gameObject.SetActive(true);
            Text t = b.GetComponentInChildren<Text>();
            t.text = btnName;

            b.onClick.AddListener(delegate {
                InvokeProjectors(btnMethod, mustBeWorking);
            });
        }
    }

    private void InitProjectors() {
        if (!willAddProjector) return;
        if (projectors != null) {
            foreach (IProjectorScript projector in projectors) {
                // FIXME
                // destroy the object here will introduce bug
                // need more investigation here
                // skip destroying the object first
                // may introduce memory issue
                //Destroy(projector.gameObject);
            }
        }
        
        projectors = new List<IProjectorScript>();
        foreach (string portName in projectorPortNames) {
            projectors.Add(GetNewProjectorScript(portName));
        }

    }

    private IProjectorScript GetNewProjectorScript (string portName, string projectorType = "custom") {
        IProjectorScript p;

        if (projectorType == "benq") {
            projectorObject.gameObject.SetActive(true);
            p = Instantiate(projectorObject);
            p.GetTransform().SetParent(projectorObject.transform.parent);
            projectorObject.gameObject.SetActive(false);

        } else {
            customProjectObject.gameObject.SetActive(true);
            p = Instantiate(customProjectObject);
            p.GetTransform().SetParent(customProjectObject.transform.parent);
            customProjectObject.gameObject.SetActive(false);
        }

        p.GetInputPortName().text = portName;
        p.GetTransform().localScale = Vector3.one;

        return p;
    }

    private void LogHandler (string message, string stackTrace, LogType type) {
        logText.text += (message + "\n");
        logText.caretPosition = logText.text.Length - 1;
    }

    private static void InitListeners () {
        if (listeners == null) {
            listeners = new List<IProjectManagerScript>();
        }

        if (listenerCallbacks == null) {
            listenerCallbacks = new List<Action<IProjectManagerScript>>();
        }
    }

    private void LoopListeners (Action<IProjectManagerScript> cb) {
        listenerCallbacks.Add(cb);
        if (listeners != null) {
            foreach (Action<IProjectManagerScript> prevCb in listenerCallbacks) {
                foreach (IProjectManagerScript _interface in listeners) {
                    prevCb(_interface);
                }
            }
            listenerCallbacks.Clear();
        }
    }

    private void LoopProjectors (Action<IProjectorScript, int> cb) {
        for (int i = 0; i < projectors.Count; i++) {
            cb(projectors[i], i);
        }
    }

    private void InvokeProjectors (string method, bool mustBeWorking = true, string callbackName = "") {
        foreach (IProjectorScript projector in projectors) {
            if (mustBeWorking && !projector.isProjectorWorking) continue;
			if (method.Contains("IE_"))
				projector.GetMonoBehaviour().StartCoroutine(method);
			else {
				projector.GetMonoBehaviour ().Invoke (method, 0);
				Debug.Log ("Run Command");
			}
        }

        if (callbackName != "") {
            // magic number to be called after the invoke
            Invoke(callbackName, 0.01f);
        }
    }

    // reset all the projects and detect for a new set
    public void DetectProjectors() {
        projectorPortNames.Clear();
        StartCoroutine(GetSupportedProjectorPortNames(portNames => {
            projectorPortNames = portNames;
            LoopListeners(i => i.OnDetectProjectors(projectorPortNames.ToArray()));

            InitProjectors();
            SavePortNames();
        }));
    }

    private IEnumerator GetSupportedProjectorPortNames (Action<List<string>> cb) {
        List<string> supportedProjectorPortNames = new List<string>();
        string[] portNames = ProjectorManagerScript.availablePortNames;
        for (int i = 0; i < portNames.Length; i++) {
            string portName = portNames[i];
            if (!portNameDetectRegex.Match(portName).Success) continue;
            if (projectorPortNamesExcluded.Contains(portName)) {
                Debug.Log(portName + " is blacklisted");
                continue;
            }
            BenqProjectorPort port = new BenqProjectorPort(portName);
            yield return port.Open();
            Debug.Log(portName + " is " + (port.IsPortSupported ? "good" : "bad"));
            if (port.IsPortSupported) {
                supportedProjectorPortNames.Add(portName);
            }
            port.Close();
        }
        cb(supportedProjectorPortNames);
    }

    private void SavePortNames() {
        string[] names = new string[projectors.Count];
        LoopProjectors((projector, index) => {
            names[index] = projector.portName;
        });
        // save the port names
        string namesStr = string.Join(",", names);
        Debug.Log("Saving: " + namesStr);
        PlayerPrefs.SetString(PREF_DEFAULT_PORT_NAMES_KEY, namesStr);
    }

    public void Turn3DProjectors () {
        InvokeProjectors("ThreeDOnHandler", false);
    }

	//Lindy Added
	public void Turn3DDualHeadProjectors () {
		InvokeProjectors ("ThreeDOnDualHeadHandler", false);
	}

    public void Turn2DProjectors() {
        InvokeProjectors("ThreeDOffHandler", false);
    }

    private IEnumerator _TurnOnProjectors () {
        InvokeProjectors("IE_PowerAnd3DOnHandler", false);
        yield return new WaitForSeconds(1);
    }

    public void TurnOnProjectors () {
        StartCoroutine("_TurnOnProjectors");        
    }

    public void UpdateProjectors () {
        InvokeProjectors("UpdateHandler");
    }

	public void ConnectProjectors () {
        InvokeProjectors("Init", false, "SavePortNames");
    }

	public void TerminateProjectors () {
        InvokeProjectors("PowerOffHandler", false);
    }

    public void CloseProjectorPorts () {
        InvokeProjectors("End", false);
    }

	public void HighResCAVE()
	{
		InvokeProjectors ("HighResCAVEInputHandler", false);
//		Debug.Log ("Run HIGHRES Command");
	}

	public void LowResCAVE()
	{
		InvokeProjectors ("LowResCAVEInputHandler", false);
//		Debug.Log ("Run LOWRES Command");
	}

    IEnumerator ShutdownSystemAfter (float seconds = 30) {
        ProgressManagerScript.DoJob("ShutdownSystemAfter");
        if (seconds < 5) {
            seconds = 5;
        }
        yield return new WaitForSeconds(seconds - 5);
        // turn of the projectors first
        TerminateProjectors();

        yield return new WaitForSeconds(5);
        // issue a command to shutdown the computer
        SystemD.Process.Start("shutdown", "/s /t 10 /c \"Projectors are off. The system will be down in 10 seconds.\" ");
        ProgressManagerScript.FinishJob("ShutdownSystemAfter");
    }

    public void ShutdownSystem (Text buttonText) {
        if (!willBeShutdown) {
            StopCoroutine(shutdownCoroutine);
            ProgressManagerScript.FinishJob("ShutdownSystemAfter");
            buttonText.text = "Shutdown";
        } else {
            shutdownCoroutine = StartCoroutine(ShutdownSystemAfter());
            buttonText.text = "Cancel\nShutdown";
        }
        willBeShutdown = !willBeShutdown;
    }

    public void Blacklist (string portName) {
        if (projectorPortNamesExcluded.IndexOf(portName) == -1) {
            projectorPortNamesExcluded.Add(portName);
        }        
    }

    public void Whitelist (string portName) {
        projectorPortNamesExcluded.RemoveAll(str => str == portName);
    }

    public void RenewBlackList(string[] portNames)
    {
        projectorPortNamesExcluded = new List<string>();
        foreach (string s in portNames) projectorPortNamesExcluded.Add(s);
        Debug.Log(String.Join(",", projectorPortNamesExcluded.ToArray()));
    }

    public List<string> GetBlacklistPortNames () {
        return projectorPortNamesExcluded;
    }

    public GameObject AddProjector (string type) {
        IProjectorScript ps = GetNewProjectorScript("", type);
        projectors.Add(ps);
        return ps.GetGameObject();
    }

    public GameObject RemoveProjectorAt (int i) {
        GameObject p = null;
        if (i < projectors.Count && i >= 0) {
            p = projectors[i].GetGameObject();
            projectors.RemoveAt(i);
        }
        return p;
    }

    public GameObject RemoveIdelProjector () {
        GameObject projector = null;
        foreach (IProjectorScript p in projectors) {
            if (p.portName == "") {
                projector = p.GetGameObject();
                projectors.Remove(p);
                break;
            }
        }
        return projector;
    }

    public static void RegisterListener (IProjectManagerScript interf) {
        if (listeners != null) {
            listeners.Add(interf);
        }
    }

    public static void UnregisterListener (IProjectManagerScript interf) {
        if (listeners != null) {
            listeners.Remove(interf);
        }
    }
}

public interface IProjectManagerScript {
    void OnDetectProjectors(string[] portNames);
    void OnLoadDefaultProjectors(string[] portNames);
}