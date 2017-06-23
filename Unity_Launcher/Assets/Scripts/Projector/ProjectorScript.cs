using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Threading;

public class ProjectorScript : MonoBehaviour, IProjectorScript {

    public InputField inputPortName, inputDelay, inputInfo;
    public Button statusConnection;

    public const float PROJECT_INIT_TO_DO_DELAY_S = 10;
    public const float PROJECTOR_ON_TO_3D_DELAY_S = 60;
    public const string PREF_PROJECTOR_ON_TO_3D_DELAY_PREFIX = "_pref_on_3d_delay_"; // KEY NAME

    private BenqProjectorPort pPort;
    private string _modelName;
    private float delayOnTo3D = PROJECTOR_ON_TO_3D_DELAY_S;

    public string portName {
        get { return this.inputPortName.text; }

        set { this.inputPortName.text = value; }
    }

    public string modelName {
        get {
            return _modelName;
        }
    }

    public bool isProjectorInit {
        get {
            return pPort != null && pPort.IsPortInitialized;
        }
    }

    public bool isProjectorWorking {
        get {
            return pPort != null && pPort.IsWorking;
        }
    }

    public bool isPortNameInvalid {
        get {
            return portName == "" || Array.IndexOf(ProjectorManagerScript.PROJECTOR_ERROR_STRINGS, portName) >= 0;
        }
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit () {
        if (!isPortNameInvalid) {
            Debug.Log("Save float");
            PlayerPrefs.SetFloat(PREF_PROJECTOR_ON_TO_3D_DELAY_PREFIX + portName, delayOnTo3D);
            PlayerPrefs.Save();
        }
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

	//	Connect Projects (initialized ports)
    public bool Init () {
        SetPortName(inputPortName.text);
        if (isPortNameInvalid) {
            return false;
        }
        pPort = new BenqProjectorPort(portName);
        pPort.Open();
        bool init = pPort.IsPortInitialized;
        //Debug.Log(/*pPort.IsPortInitialized*/);

        SetInfoText(portName + (pPort.IsWorking ? "" : " not") + " connected");
        ColorBlock colorBlock = statusConnection.colors;
        Color nextColor = pPort.IsWorking ? Color.green : Color.red;
        colorBlock.normalColor = nextColor;
        colorBlock.highlightedColor = nextColor;
        statusConnection.colors = colorBlock;
        return true;
    }

	// ?
    public void OnDelayChangeHandler () {
        float delay;
        try {
            delay = float.Parse(inputDelay.text);
            delayOnTo3D = delay;
            //Debug.Log(delay);
        } catch (FormatException e) {
            Debug.Log(e.Message);
        }
    }

    public void SetPortName (string portName) {
        this.portName = portName;
        this.inputPortName.text = portName;

        if (!isPortNameInvalid) {
            delayOnTo3D = PlayerPrefs.GetFloat(PREF_PROJECTOR_ON_TO_3D_DELAY_PREFIX + portName, delayOnTo3D);
            inputDelay.text = delayOnTo3D.ToString();
        } else {
            this.portName = "";
        }
    }

    public void SetInfoText (string text) {
        if (this.inputInfo != null) {
            inputInfo.text = text;
        }
    }

	public void End () {
        if (pPort != null)
		    pPort.Close ();
	}

    private void TryInitAndDo (Action cb) {
        if (!isProjectorInit) Init();

        if (pPort != null) {
            cb();
        }
    }

    private IEnumerator TryInitAndDoAsync(Action cb, float actionDelaySec = PROJECT_INIT_TO_DO_DELAY_S) {
        ProgressManagerScript.DoJob("TryInitAndDoAsync:" + cb.Method.Name);
        bool didInit = false;
        if (!isProjectorInit) {
            Init();
            didInit = true;
        }

        if (pPort != null) {
            float actualWait = didInit ? actionDelaySec : 0.5f;
            Debug.Log("Wait for " + actualWait);
            yield return new WaitForSeconds(actualWait);
            cb();
        }
        ProgressManagerScript.FinishJob("TryInitAndDoAsync:" + cb.Method.Name);
    }

    public IEnumerator IE_PowerAnd3DOnHandler () {
        ProgressManagerScript.DoJob("IE_PowerAnd3DOnHandler");
        Debug.Log(portName + " Async init? " + isProjectorInit);
        bool didInit = false;
        if (!isProjectorInit) {
            didInit = Init();
        }

        if (pPort != null) {
            Debug.Log(portName + " Async power + 3d");
            yield return new WaitForSeconds(didInit ? PROJECT_INIT_TO_DO_DELAY_S : 0.5f);
            pPort.PowerOn();
            yield return new WaitForSeconds(delayOnTo3D);
            Debug.Log(portName + " Async 3d");
            pPort._3DEnable();
            yield return new WaitForSeconds(1f);
            UpdateHandler();
        }
        ProgressManagerScript.FinishJob("IE_PowerAnd3DOnHandler");
    }

	public void PowerOnHandler () {
        StartCoroutine(TryInitAndDoAsync(() => {
            Debug.Log("now power on...");
            pPort.PowerOn();
        }));
	}

	public void PowerOffHandler () {
        StartCoroutine(TryInitAndDoAsync(() => {
            Debug.Log("now power off...");
            pPort.PowerOff();
        }));
    }

    public void ThreeDOnHandler() {
        StartCoroutine(TryInitAndDoAsync(() => {
            Debug.Log("now 3d on frame sequential...");
            pPort._3DEnable();
        }));
    }

    public void ThreeDOffHandler () {
        StartCoroutine(TryInitAndDoAsync(() => {
            Debug.Log("now 3d off...");
            pPort._3DDisable();
        }));
    }

	// Lindy Added
	public void ThreeDOnDualHeadHandler() {
		StartCoroutine (TryInitAndDoAsync (() => {
			Debug.Log ("now 3d on dual head...");
			pPort._3DEnable ();
		}));
	}

	public void HighResCAVEInputHandler()
	{
		Debug.Log ("BARCO DPORT1");
	}

	public void LowResCAVEInputHandler()
	{
	}
//	public void HighResCAVE134Handler()
//	{
//		StartCoroutine (TryInitAndDoAsync (() => {
//			Debug.Log ("Setting HighResCAVE 1,3,4...");
//			// ...
//		}));
//	}
//
//	public void HighResCAVE2Handler()
//	{
//		StartCoroutine (TryInitAndDoAsync (() => {
//			Debug.Log ("Setting HighResCAVE 2...");
//			// ...
//		}));
//	}
//
//	public void LowResCAVE134Handler()
//	{
//		StartCoroutine (TryInitAndDoAsync (() => {
//			Debug.Log ("Setting LowResCAVE 1,3,4...");
//			// ...
//		}));
//	}
//
//	public void LowResCAVE2Handler()
//	{
//		StartCoroutine (TryInitAndDoAsync (() => {
//			Debug.Log ("Setting LowResCAVE 2...");
//			// ...
//		}));
//	}


    public void UpdateStatus () {
        SetInfoText(String.Format("{0} : {1} : {2}", _modelName, pPort.GetPower(), pPort.Get3DStatus()));
    }

    public void UpdateHandler () {
        StartCoroutine(pPort.GetModelNameAsync((modelName) => {
            this._modelName = modelName;
            UpdateStatus();
        }));
    }
}
