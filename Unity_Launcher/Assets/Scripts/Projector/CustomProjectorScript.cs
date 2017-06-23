using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomProjectorScript : MonoBehaviour, IProjectorScript {

	// Get the imputPortName, delay and ConfigFilePath from the inputField from user GUI
    public InputField inputPortName, inputDelay, inputConfigPath;
    public Button statusConnection;

    public bool isDebugMode = false;

    private ProjectorConfig config;
    private ProjectorPort port;

    private Queue<string> actions;

    private bool lockFlag = false;

	// Use this for initialization
	void Start () {
	    SetConfigPath(projectorConfigFile);
	    actions = new Queue<string>();
	}

	// Update is called once per frame
	void Update () {
	    if (isDebugMode) {
	        if (Input.GetKeyUp(KeyCode.Alpha1)) {
	            Init();
	        } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
	            PowerOnHandler();
	        } else if (Input.GetKeyUp(KeyCode.Alpha3)) {
	            PowerOffHandler();
	        } else if (Input.GetKeyUp(KeyCode.Alpha4)) {
	            ThreeDOnHandler();
	        } else if (Input.GetKeyUp(KeyCode.Alpha5)) {
	            ThreeDOffHandler();
	        } else if (Input.GetKeyUp(KeyCode.Alpha6)) {
	            StartCoroutine(IE_PowerAnd3DOnHandler());
	        }
	    }

	    if (port != null && port.IsWorking && actions.Count > 0) {
	        StartCoroutine(ConsumeAction());
	    }
	}

    IEnumerator ConsumeAction() {
        if (!lockFlag) {
            lockFlag = true;

            string nextAction = actions.Dequeue();
            Debug.Log("Next action: " + nextAction);
            if (nextAction.StartsWith(ProjectorConfig.ConfigCommandPrefix)) {
                string reply = port.SendCommand(config.Get(nextAction), ProjectorConfig.ConfigCommandSeparators);
                Debug.Log("Got (" + reply.Length + ") : " + reply);
            } else if (nextAction.StartsWith(ProjectorConfig.ConfigWaitPrefix)) {
                yield return new WaitForSeconds(ProjectorConfig.WaitStringToFloat(nextAction));
                Debug.Log("Done: " + nextAction);
            }

            lockFlag = false;
        }
    }

    private void DispatchAction(string action) {
        Debug.Log("Pending: " + action);
        actions.Enqueue(action); // Add action to the Queue
    }

    private void SetConfigPath(string configPath) {
        if (projectorConfigFile != "") {
            config = new ProjectorConfig(projectorConfigFile);
        }
    }

    public string portName {
        get { return inputPortName.text; }
        set { inputPortName.text = value; }
    }

    public string projectorConfigFile {
        get { return inputConfigPath.text; }
    }

    public bool isProjectorWorking {
        get { return port != null; }
    }

    public InputField GetInputPortName() {
        return inputPortName;
    }

    public InputField GetInputDelay() {
        return inputDelay;
    }

    public Button GetStatusConnection() {
        return statusConnection;
    }

    public Transform GetTransform() {
        return transform;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    public MonoBehaviour GetMonoBehaviour() {
        return this;
    }

    public bool Init() {
        if (portName == "") return false;

        // ensure the config file path
        if (config == null || config.configFilePath != projectorConfigFile) {
            Debug.Log("Not same config path. Parse again.");
            config = new ProjectorConfig(projectorConfigFile);
        }

        if (!config.IsReady) return false;

        port = new ProjectorPort(
            portName,
            config.GetInt(ProjectorConfig.ConfigAttrBuadrate, ProjectorPort.PROJECTOR_BAUD_RATE),
            config.Get(ProjectorConfig.ConfigAttrParityCheck, ProjectorPort.PROJECTOR_PARITY_CHECK),
            config.GetInt(ProjectorConfig.ConfigAttrDataBits, ProjectorPort.PROJECTOR_DATA_BITS),
            config.Get(ProjectorConfig.ConfigAttrStopBit, ProjectorPort.PROJECTOR_STOP_BIT)
        );

        bool isOpen = port.Open();
        ColorBlock colorBlock = statusConnection.colors;
        Color nextColor = port.IsWorking ? Color.green : Color.red;
        colorBlock.normalColor = nextColor;
        colorBlock.highlightedColor = nextColor;
        statusConnection.colors = colorBlock;

        return isOpen;
    }

    public void End() {
        if (port != null) {
            port.Close();
        }
    }

    public void PowerOnHandler() {
        DispatchAction(ProjectorConfig.ConfigCommandPowerOn);
        DispatchAction(ProjectorConfig.FloatToWaitString(10));
    }

    public void PowerOffHandler() {
        DispatchAction(ProjectorConfig.ConfigCommandPowerOff);
        DispatchAction(ProjectorConfig.FloatToWaitString(10));
    }

    public void ThreeDOnHandler() {
        if (ProjectorPart.mode.Contains("DH")) ThreeDOnDualHeadHandler();
        else DispatchAction(ProjectorConfig.ConfigCommand3DOn);
    }

    public void ThreeDOffHandler() {
        DispatchAction(ProjectorConfig.ConfigCommand3DOff);
    }

	// Lindy Add 
	public void ThreeDOnDualHeadHandler() {
		// check if the current projector is BARCO or not, if yes:
		if (inputConfigPath.text.Contains ("barco")) {
			Debug.Log ("BARCO RUNS DUALHEAD");
			DispatchAction (ProjectorConfig.ConfigCommand3DOn_dualhead);
		}
		// if not, ignore this function.
	}

	public void HighResCAVEInputHandler()
	{
		// check if the current projector is BARCO or not, if yes:
		if (inputConfigPath.text.Contains ("barco")) {
			Debug.Log ("BARCO DPORT1");
			DispatchAction (ProjectorConfig.ConfigCommand_DPort1);
		}
		// if not, ignore this function.
	}

	public void LowResCAVEInputHandler()
	{
		// check if the current projector is BARCO or not, if yes:
		if (inputConfigPath.text.Contains ("barco")) {
			Debug.Log ("BARCO HDMI");
			DispatchAction (ProjectorConfig.ConfigCommand_HDMI);
		}
		// if not, ignore this function.
	}
		


    public virtual void UpdateHandler() {
        Debug.Log("Not implemented in CustomProjectorScript");
    }

    public IEnumerator IE_PowerAnd3DOnHandler() {
        bool isInit = port != null || Init();
        if (isInit) {
            DispatchAction(ProjectorConfig.ConfigCommandPowerOn);
            DispatchAction(ProjectorConfig.FloatToWaitString(30));
            DispatchAction(ProjectorConfig.ConfigCommand3DOn);
        }
       
        yield return 0;
    }
}
