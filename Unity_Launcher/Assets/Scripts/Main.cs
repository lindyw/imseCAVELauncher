using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public Transform loader;
    public string defaultMVRVer = "1.6.1.f6";

    public bool needLogin = false;
    public string ac; // account 
    public string pw; // password
    public Color style = Color.white;
    public Color barColor = Color.white;
    public Color[] buttonColor = { Color.white, Color.grey, Color.black, Color.gray };
    public Color[] iconColor = { Color.white, Color.grey, Color.black, Color.gray };
    public Color scrollBGColor = Color.white;
    public Color[] scrollBarColor = { Color.white, Color.grey, Color.black, Color.gray };
    public Color scrollBarBGColor = Color.grey;

    int numOfProjectors = 0;
    int numOfPrograms = 1;

    public Image[] UIWidnows;
    public Image[] UIBars;
    public Button[] UIButtons;
    public Button[] UIIcons;
    public Image[] UIScrolls;
    public Image[] UIScrollBG;
    public Scrollbar[] UIScrollBars;
    public GameObject projectorTmp;
    public GameObject programTmp;
    public GameObject buttonTmp;
    public GameObject settingBtn;
    public GameObject logoutBtn;
    public GameObject[] arrowBtns;
    public RectTransform loadingContents;
    public Image hightLight;

    List<GameObject> projectorStore = new List<GameObject>();
    List<GameObject> programStore = new List<GameObject>();
    List<GameObject> btnStore = new List<GameObject>();
    Projector[] projectors = new Projector[0];
    Program[] programs = new Program[0];
    public GameObject[] startSet;
    public RectTransform[] scrollsContent;
    Vector3[] contentsPos = new Vector3[2];

    int page = 1;
    int confirmMsg = 0;
    int alertMsg = 0;
    int loadMsg = 0;
    int startMsg = -1;
    int settingMsg = 0;
    int endMsg = -1;
    public Text[] windowsMsg;
    public Image black;
    public RawImage[] modifiable;
    public Text[] setBtns;

    public ECUIController[] pages;
    public ECUIController[] windows;
    public AudioSource[] sfx;
    public ECAudioController vo;

    public ECTabSelect tabSelector;
    public List<Selectable> inputP0 = new List<Selectable>();
    public List<Selectable> inputP1 = new List<Selectable>();
    public List<Selectable> inputP2 = new List<Selectable>();
    public List<Selectable> inputP3 = new List<Selectable>();

    public ProjectorPart projectPart;
//	public GameObject udpTesting;
	public ProjectorConfig3D projectorConfig3D;
	public ProjectorManagerScript projectorManager;
    GameObject[] tmp = { null, null };

    float loadingBuffer = 0;
    string currentPath;
    string currentProgram;
    string currentEnvironment = "";
    bool currentFullScreen = false;
    public static bool currentAutoShut = false;
    int selectedProgram = 0;

    float btnWidth = 0;
    float btnPos = 0;
    public Transform btnPanel;
    float delay = 0;
    bool isQuit = false;
    bool isIni = false;
    bool isRun = false;

    int sMon = 1;
    int sWidth = 1600;
    int sHeight = 900;

    Dictionary<string, string> environment = new Dictionary<string, string>();
    string pathDir = "C:\\windows\\system32;C:\\windows;C:\\windows\\System32\\Wbem;C:\\windows\\System32\\WindowsPowerShell\\v1.0\\;C:\\Program Files (x86)\\";
    string programList = "../Resources/Text/ProgramList.txt";
    // getter / setter
    public int projectorStoreCount {
        get {
            return this.projectorStore.Count;
        }
    }
    
    // Use this for initialization
    void Start ()
    {
		InputFieldAddMask ();
        contentsPos[0] = scrollsContent[0].localPosition;
        contentsPos[1] = scrollsContent[1].localPosition;
        needLogin = !needLogin;
        GetData();

        //PlayerPrefs.DeleteAll();
        InvokeRepeating("LoaderRotate", 0, Time.deltaTime * 3);

        projectorTmp.SetActive(false);
        programTmp.SetActive(false);
        buttonTmp.SetActive(false);

        if (PlayerPrefs.GetString("Projectors").Length > 0) numOfProjectors = int.Parse(PlayerPrefs.GetString("Projectors"));
        numOfPrograms = ECFile.ReadLines(programList).Length;
        currentFullScreen = (PlayerPrefs.GetInt("Full") <= 0);
        currentAutoShut = (PlayerPrefs.GetInt("Auto") <= 0);
        needLogin = (PlayerPrefs.GetInt("Login") <= 0);
        if (PlayerPrefs.GetString("AC").Length > 0) ac = PlayerPrefs.GetString("AC");
        if (PlayerPrefs.GetString("PW").Length > 0) pw = PlayerPrefs.GetString("PW");

        RenewProjectors(0);
        ConfirmRenewProjectors();
        RenewPrograms(0);
        ConfirmRenewPrograms();

        Invoke("Ini", Time.deltaTime * 10);
        StartCoroutine(FadeOut());

    }

    void Ini()
    {
        LoginRequirement();

        page = numOfProjectors > 0 ? 1 : (needLogin ? 0 : 2);   //Projector -> Login -> Program
        for (int i = 0; i < pages.Length; i++)
        {
            if (i == page) pages[i].FadeIn(99999);
            else pages[i].FadeOut(99999);
        }
        for (int i = 0; i < windows.Length; i++) windows[i].FadeOut(99999);
        if (page > 0)
        {
            tabSelector.SetTabObjects(inputP1);
            if (page == 1) StartCoroutine(LoadMsg(1, 0.5f));
        }
        else tabSelector.SetTabObjects(inputP0);

        WindowMode(0.5f);
        AutoShut();

        string[] upperVO = new string[vo.nameIndex.Count];
        vo.nameIndex.Keys.CopyTo(upperVO, 0);
        for(int i = 0; i < upperVO.Length; i++)
        {
            vo.nameIndex.Remove(upperVO[i]);
            vo.nameIndex.Add(upperVO[i].ToUpper(), i);
        }
    }

	void InputFieldAddMask()
	{
		InputField[] inputFields = (InputField[]) GameObject.FindObjectsOfType (typeof(InputField));
		foreach (InputField i in inputFields)
			if (i.gameObject.GetComponent<FixInputSelectionMasking>() == null) i.gameObject.AddComponent<FixInputSelectionMasking>();
	}

    void GetData()
    {
		// UI color settings
        string[] colors = ECFile.ReadLines("../Resources/Text/Color.txt");
        for (int i = 0; i < colors.Length; i++)
        {
            string[] tmp = ECCommons.Separate(", ", colors[i]);
            if (tmp.Length > 4)
            {
                Color c = (Color)new Color32(byte.Parse(tmp[1]), byte.Parse(tmp[2]), byte.Parse(tmp[3]), byte.Parse(tmp[4]));
                switch (tmp[0])
                {
                    case "Style": style = c; break;
                    case "BarColor": barColor = c; break;
                    case "ButtonColorA": buttonColor[0] = c; break;
                    case "ButtonColorB": buttonColor[1] = c; break;
                    case "ButtonColorC": buttonColor[2] = c; break;
                    case "ButtonColorD": buttonColor[3] = c; break;
                    case "IconColorA": iconColor[0] = c; break;
                    case "IconColorB": iconColor[1] = c; break;
                    case "IconColorC": iconColor[2] = c; break;
                    case "IconColorD": iconColor[3] = c; break;
                    case "ScrollBGColor": scrollBGColor = c; break;
                    case "ScrollBarColor": scrollBarBGColor = c; break;
                    case "ScrollBarBGColorA": scrollBarColor[0] = c; break;
                    case "ScrollBarBGColorB": scrollBarColor[1] = c; break;
                    case "ScrollBarBGColorC": scrollBarColor[2] = c; break;
                    case "ScrollBarBGColorD": scrollBarColor[3] = c; break;
                    case "BGColor": modifiable[0].color = c; break;
                }
            }
        }
        for (int i = 0; i < UIWidnows.Length; i++) UIWidnows[i].color = style;
        for (int i = 0; i < UIBars.Length; i++) UIBars[i].color = barColor;
        for (int i = 0; i < UIScrolls.Length; i++) UIScrolls[i].color = scrollBGColor;
        for (int i = 0; i < UIScrollBG.Length; i++) UIScrollBG[i].color = scrollBarBGColor;
        for (int i = 0; i < UIButtons.Length; i++)
        {
            ColorBlock c = UIButtons[i].colors;
            c.normalColor = buttonColor[0];
            c.highlightedColor = buttonColor[1];
            c.pressedColor = buttonColor[2];
            c.disabledColor = buttonColor[3];
            UIButtons[i].colors = c;
        }
        for (int i = 0; i < UIIcons.Length; i++)
        {
            ColorBlock c = UIIcons[i].colors;
            c.normalColor = iconColor[0];
            c.highlightedColor = iconColor[1];
            c.pressedColor = iconColor[2];
            c.disabledColor = iconColor[3];
            UIIcons[i].colors = c;
        }
        for (int i = 0; i < UIScrollBars.Length; i++)
        {
            ColorBlock c = UIScrollBars[i].colors;
            c.normalColor = scrollBarColor[0];
            c.highlightedColor = scrollBarColor[1];
            c.pressedColor = scrollBarColor[2];
            c.disabledColor = scrollBarColor[3];
            UIScrollBars[i].colors = c;
        }

        if (ECFile.FileExists("../Modifiable/bg.png")) modifiable[0].texture = ECFile.LoadImage("../Modifiable/bg.png");
        if (ECFile.FileExists("../Modifiable/logo.png")) modifiable[1].texture = ECFile.LoadImage("../Modifiable/logo.png");
        // config paths for PowerShell and MiddleVR
        string[] data = ECFile.ReadLines("../Resources/Text/Config.txt");
        if (data.Length > 0)
        {
            environment.Clear();
            pathDir = ECCommons.Separate(", ", data[0])[1];
        }
        for (int i = 1; i < data.Length; i++)
        {
            string[] tmp = ECCommons.Separate(", ", data[i]);
            environment.Add(tmp[0], pathDir + tmp[1]);
            if (tmp.Length > 2) defaultMVRVer = tmp[0];
        }
        string[] dps = ECFile.ReadLines("../Resources/Text/Display.txt");
        for (int i = 0; i < dps.Length; i++)
        {
            string[] tmp = ECCommons.Separate(", ", dps[i]);
            switch (tmp[0])
            {
                case "Monitor": sMon = int.Parse(tmp[1]); break;
                case "Resolution": sWidth = int.Parse(tmp[1]); sHeight = int.Parse(tmp[2]); break;
                case "3DMode": ProjectorPart.mode = tmp[1]; break;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log(ECUIEvent.overlappedObject);
        if (ECUIEvent.overlappedObject != tmp[0])
        {
            tmp[0] = ECUIEvent.overlappedObject;
            if (ECUIEvent.overlappedObject)
            {
                switch (ECUIEvent.overlappedObject.tag)
                {
                    case "Btn": sfx[0].Play(); break;
                }
            }
        }
        else if (ECUIEvent.pressedObject != tmp[1])
        {
            tmp[1] = ECUIEvent.pressedObject;
            if (ECUIEvent.pressedObject)
            {
                switch (ECUIEvent.pressedObject.tag)
                {
                    case "Btn": sfx[1].Play(); break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            if (settingMsg != 0) CloseSetting(false);
            else if (confirmMsg != 0) CloseConfirm(false);
            else if (alertMsg != 0) CloseAlert();
            else if (startMsg != -1) CloseStarter(false);
            else if (endMsg != -1) CloseEnder();
            else if (page < 3) OpenEnder();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (settingMsg != 0) CloseSetting(true);
            else if (confirmMsg != 0) CloseConfirm(true);
            else if (alertMsg != 0) CloseAlert();
            else if (startMsg != -1) CloseStarter(true);
            else if (endMsg != -1) CloseEnder();
            else
            {
                switch (page)
                {
                    case 0: Login(); break;
                    case 2: OpenStarter(selectedProgram); break;
                }
            }
        }
        else if (Input.GetKey(KeyCode.LeftAlt)) {
            if (Input.GetKeyDown(KeyCode.W)) { WindowMode(0); }
            else if (Input.GetKey(KeyCode.RightShift))
            {
                // Basic Settings
                if (Input.GetKeyDown(KeyCode.Alpha1)) { OpenSetting(1); } // projectorConfig3D.CheckProjectors_Barco(UIButtons[35]); }
				// Advanced Settings (Projectors) 
				else if (Input.GetKeyDown(KeyCode.Alpha2)) { OpenSetting(2); } 
				// Developer Settings (Add Applications)
				else if (Input.GetKeyDown(KeyCode.Alpha3)) { OpenSetting(3); } 
				// Turn on/off login requirement
                else if (Input.GetKeyDown(KeyCode.L)) { OpenLoad(12); } 
				// Turn on/off currentAutoShut
				else if (Input.GetKeyDown(KeyCode.S)) { OpenLoad(13); } 
				// Update Login Data
                else if (Input.GetKeyDown(KeyCode.K)) { OpenLoad(14); } 
                else if (Input.GetKeyDown(KeyCode.Delete)) PlayerPrefs.DeleteAll();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5)) GetData();

        if (loadingBuffer > 0) loadingBuffer -= Time.deltaTime;
        else if (loadMsg != 0) CloseLoad();

        if (Application.runInBackground != (isQuit || !isRun)) Application.runInBackground = isQuit || !isRun;
        if (page > 1 != settingBtn.activeInHierarchy) settingBtn.SetActive(page > 1); // && numOfProjectors > 0
        if ((needLogin && page > 1) != logoutBtn.activeInHierarchy) logoutBtn.SetActive(needLogin && page > 1);
        if ((page == 2 && btnPos < btnWidth) != arrowBtns[0].activeInHierarchy) arrowBtns[0].SetActive(page == 2 && btnPos < btnWidth);
        if ((page == 2 && btnPos > 0) != arrowBtns[1].activeInHierarchy) arrowBtns[1].SetActive(page == 2 && btnPos > 0);
        if (Vector3.Distance(btnPanel.localPosition, new Vector3(-btnPos, 0, 0)) > 32 != arrowBtns[2].activeInHierarchy) arrowBtns[2].SetActive(Vector3.Distance(btnPanel.localPosition, new Vector3(-btnPos, 0, 0)) > 32);
        if (btnPanel.localPosition != new Vector3(-btnPos, 0, 0)) btnPanel.localPosition = Vector3.Lerp(btnPanel.localPosition, new Vector3(-btnPos, 0, 0), Time.deltaTime * 5);

        if (delay > 0) delay -= Time.deltaTime;
        else if (delay <= 0 && Directory.Exists("../Resources/AppClosed"))
        {
            delay = 1;
            Directory.Delete("../Resources/AppClosed");
            if (page == 3)
            {
                currentPath = "";
                ChangePage(2);
                isRun = false;
            }
        }

        if (page == 2 && settingMsg <= 0 && confirmMsg <= 0 && loadMsg <= 0 && alertMsg <= 0 && startMsg < 0 && endMsg < 0)
        {
            if (btnPos < btnWidth && NextKey())
            {
                ShowSlide(1);
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.JoystickButton5))
                {
                    vo.Stop();
                    vo.Play(programs[selectedProgram].name.ToUpper());
                }
            }
            else if (btnPos > 0 && PrevKey())
            {
                ShowSlide(-1);
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.JoystickButton4))
                {
                    vo.Stop();
                    vo.Play(programs[selectedProgram].name.ToUpper());
                }
            }
        }
        else if (settingMsg >= 2)
        {
            float contentHeight = settingMsg == 2 ? 160 : 80;
            if (NextKey()) contentsPos[settingMsg - 2] += new Vector3(0, contentHeight, 0);
            else if (PrevKey()) contentsPos[settingMsg - 2] -= new Vector3(0, 80, 0);
            else if (Input.GetKey(KeyCode.Mouse0)) contentsPos[settingMsg - 2] = scrollsContent[settingMsg - 2].localPosition;
            if (contentsPos[settingMsg - 2].y < 0) contentsPos[settingMsg - 2] = Vector3.zero;
            else if (contentsPos[settingMsg - 2].y > scrollsContent[settingMsg - 2].sizeDelta.y - 360)
            {
                if(scrollsContent[settingMsg - 2].sizeDelta.y >= 360) contentsPos[settingMsg - 2] = new Vector3(0, scrollsContent[settingMsg - 2].sizeDelta.y - 360, 0);
                else contentsPos[settingMsg - 2] = Vector3.zero;
            }
        }

        if (settingMsg == 3) hightLight.color = Input.GetKey(KeyCode.LeftShift) ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 0.25f);
        for(int i = 0; i < 2; i++ ) if (contentsPos[i] != scrollsContent[i].localPosition) scrollsContent[i].localPosition = Vector3.Lerp(scrollsContent[i].localPosition, contentsPos[i], Time.deltaTime * 20);
    }

    bool NextKey()
    {
        return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetAxis("Mouse ScrollWheel") < 0f;
    }

    bool PrevKey()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetAxis("Mouse ScrollWheel") > 0f;
    }

    void LoaderRotate()
    {
        loader.Rotate(new Vector3(0, 0, -30));
    }

    public void RenewBenqProjector() {
        _RenewProjectors(1, null, "benq");
    }

    public void RenewProjectors(int add) {
        _RenewProjectors(add);
    }

	// Update list : Add and remove projectors
    public void _RenewProjectors(int add, string[] names = null, string type = "custom")
    {
        if (add < 0 && numOfProjectors >= -add || add > 0) numOfProjectors += add;
        if (numOfProjectors > projectorStore.Count)
        {
            for (int i = projectorStore.Count; i < numOfProjectors; i++)
            {
                // add new projector here
                GameObject p = projectPart.projectorManager.AddProjector(type);
                p.SetActive(true);
                p.transform.SetParent(projectorTmp.transform.parent);
                p.transform.localPosition = new Vector3(599.5f, -160 * i, 0);
                projectorStore.Add(p);
                if (add > 0)
                {
                    Transform t = p.transform;
                    inputP2.Add(t.GetChild(1).GetComponent<InputField>()); inputP2.Add(t.GetChild(3).GetComponent<InputField>()); inputP2.Add(t.GetChild(2).GetComponent<InputField>());
                    tabSelector.SetTabObjects(inputP2, tabSelector.currentID);
                }
            }
        }
        else if (numOfProjectors < projectorStore.Count)
        {
            for (int i = projectorStore.Count - 1; i >= numOfProjectors; i--)
            {
                // remove a projector here
                Destroy(projectorStore[i]);
                projectPart.projectorManager.RemoveProjectorAt(i);
                projectorStore.RemoveAt(i);
                inputP2.RemoveRange(inputP2.Count - 3, 3);
                if (inputP2.Count > 0) tabSelector.SetTabObjects(inputP2, tabSelector.currentID);
                else tabSelector.SetTabObjects(new Selectable[0]);
            }
        }

        // load default ports
        if (add == 0 && names == null)
        {
            for (int i = 0; i < numOfProjectors; i++) {
                Transform t = projectorStore[i].transform;
                if (PlayerPrefs.GetString("Projector_" + i + "_Name").Length > 0) t.GetChild(1).GetComponent<InputField>().text = PlayerPrefs.GetString("Projector_" + i + "_Name");
                else t.GetChild(1).GetComponent<InputField>().text = "";
                if (PlayerPrefs.GetString("Projector_" + i + "_Delay").Length > 0) t.GetChild(2).GetComponent<InputField>().text = PlayerPrefs.GetString("Projector_" + i + "_Delay");
                else t.GetChild(2).GetComponent<InputField>().text = "";
                if (PlayerPrefs.GetString("Projector_" + i + "_ConfigPath").Length > 0) {
                    t.GetChild(3).GetComponent<InputField>().text = PlayerPrefs.GetString("Projector_" + i + "_ConfigPath");
                } else {
                    t.GetChild(3).GetComponent<InputField>().text = "";
                }
            }
        } else if (names != null) {
            for (int i = 0; i < Math.Min(numOfProjectors, names.Length); i++) {
                Transform t = projectorStore[i].transform;
                t.GetChild(1).GetComponent<InputField>().text = names[i];
                t.GetChild(2).GetComponent<InputField>().text = 60.ToString();
                t.GetChild(3).GetComponent<InputField>().text = "";
            }
        }

        projectorTmp.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(1173, 20 + 160 * projectorStore.Count);
    }
    void ConfirmRenewProjectors()
    {
        inputP2.Clear();
        for (int i = projectorStore.Count; i < projectors.Length; i++)
        {
            PlayerPrefs.DeleteKey("Projector_" + i + "_Name");
            PlayerPrefs.DeleteKey("Projector_" + i + "_Delay");
            PlayerPrefs.DeleteKey("Projector_" + i + "_ConfigPath");
        }
        projectors = new Projector[numOfProjectors];
        for (int i = 0; i < projectors.Length; i++)
        {
            Transform t = projectorStore[i].transform;
            projectors[i] = new Projector();
            InputField ipf = t.GetChild(1).GetComponent<InputField>();
            inputP2.Add(ipf);
            projectors[i].name = ipf.text;
            ipf = t.GetChild(2).GetComponent<InputField>();
            inputP2.Add(ipf);
            projectors[i].delay = ipf.text;
            ipf = t.GetChild(3).GetComponent<InputField>();
            inputP2.Add(ipf);
            projectors[i].configPath = ipf.text;
            PlayerPrefs.SetString("Projector_" + i + "_Name", projectors[i].name);
            PlayerPrefs.SetString("Projector_" + i + "_Delay", projectors[i].delay);
            PlayerPrefs.SetString("Projector_" + i + "_ConfigPath", projectors[i].configPath);
        }
        PlayerPrefs.SetString("Projectors", "" + numOfProjectors);
        ProjectorPart.numOfProjectors = numOfProjectors;
    }
		
	// When open Developer Mode, renew programs and their information [RenewPrograms(0)]
    public void RenewPrograms(int add)
    {

        if (add < 0 && numOfPrograms > -add || add > 0) numOfPrograms += add;
        else if (add == 0) hightLight.transform.localPosition = new Vector3(585, 500, 0);
        if (numOfPrograms > programStore.Count)
        {
            for (int i = programStore.Count; i < numOfPrograms; i++)
            {
                GameObject p = GameObject.Instantiate(programTmp);
                p.SetActive(true);
                p.transform.SetParent(programTmp.transform.parent);
                p.transform.localPosition = new Vector3(599.5f, -80 * i, 0);
                programStore.Add(p);
                if (add > 0)
                {
                    Transform t = p.transform;
                    inputP3.Add(t.GetChild(1).GetComponent<InputField>()); inputP3.Add(t.GetChild(2).GetComponent<InputField>());
                    inputP3.Add(t.GetChild(3).GetComponent<InputField>()); inputP3.Add(t.GetChild(4).GetComponent<InputField>());
                    inputP3.Add(t.GetChild(5).GetComponent<InputField>());
                    tabSelector.SetTabObjects(inputP3, tabSelector.currentID);
                }
            }
        }
        else if (numOfPrograms < programStore.Count)
        {
            if (ECUIEvent.releasedObject)
            {
                if (ECUIEvent.releasedObject.transform.parent != null)
                {
                    int f = -1;
                    if(!ECUIEvent.releasedObject.transform.parent.name.Contains("Set")) int.TryParse(ECUIEvent.releasedObject.transform.parent.name, out f);
                    ECUIEvent.releasedObject = null;
                    if (f >= 0)
                    {
                        for (int i = f + 1; i < programStore.Count; i++)
                        {
                            Transform t1 = programStore[i - 1].transform;
                            Transform t2 = programStore[i].transform;
                            for (int t = 1; t <= 5; t++) t1.GetChild(t).GetComponent<InputField>().text = t2.GetChild(t).GetComponent<InputField>().text;
                        }
                    }
                }
            }
            for (int i = programStore.Count - 1; i >= numOfPrograms; i--)
            {
                Destroy(programStore[i]);
                programStore.RemoveAt(programStore.Count - 1);
                inputP3.RemoveRange(inputP3.Count - 5, 5); 
                if (inputP3.Count > 0) tabSelector.SetTabObjects(inputP3, tabSelector.currentID);
                else tabSelector.SetTabObjects(new Selectable[0]);
            }
        }
        string[] pList = ECFile.ReadLines(programList);
        for (int i = 0; i < numOfPrograms; i++)
        {
            Transform t = programStore[i].transform;
            t.name = "" + i;
            if (add == 0)
            {
                string[] data = { "", "", "", "", "" };
                if(i < pList.Length) data = ECCommons.Separate(", ", pList[i]);
				for (int j = 1; j <= Mathf.Min (data.Length, 5); j++) 
				{
					t.GetChild (j).GetComponent<InputField> ().text = data [j - 1];
//					if (j < 4)
//						t.GetChild (j).GetComponent<InputField> ().text = data [j - 1];
//					else {
						//TODO: select the correct value index choice from DropDownList
//					}
				}
            }
        }
        programTmp.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(1173, 20 + 80 * programStore.Count);
    }
    void ConfirmRenewPrograms()
    {
        inputP3.Clear();
        List<string> data = new List<string>();
        programs = new Program[programStore.Count];
        string[] pList = ECFile.ReadLines(programList);
        for (int i = 0; i < programs.Length; i++)
        {
            Transform t = programStore[i].transform;
            programs[i] = new Program();
            InputField ipf;
            int[] p = { 0, 1, 2, 3, 4 };
//            for (int j = 0; j < 4; j++)
//            {
//                ipf = t.GetChild(p[j] + 1).GetComponent<InputField>();
//                programs[i].Set(p[j], ipf.text);
//                inputP3.Add(ipf);
//            }
			for (int j = 0; j < 5; j++) {
				ipf = t.GetChild (p [j] + 1).GetComponent<InputField> ();
				programs [i].Set (p [j], ipf.text);
				inputP3.Add (ipf);
			}
//            string icon = "";
//            if(pList.Length > i && pList[i].Length > 4) icon = ECCommons.Separate(", ", pList[i])[4];
//            programs[i].Set(p[4], icon);
			data.Add(programs[i].name + ", " + programs[i].path + ", " + programs[i].type + ", " + programs[i].mvrVersion + ", " + programs[i].icon);
        }
        ECFile.WriteLines(programList, data.ToArray());
        if (numOfPrograms > btnStore.Count)
        {
            for (int i = btnStore.Count; i < numOfPrograms; i++)
            {
                GameObject p = GameObject.Instantiate(buttonTmp);
                p.SetActive(true);
                p.transform.SetParent(buttonTmp.transform.parent);
                p.transform.localPosition = new Vector3(450 * i, 0, 0);
                btnStore.Add(p);
            }
        }
        else
        {
            for (int i = btnStore.Count - 1; i >= numOfPrograms; i--)
            {
                Destroy(btnStore[i]);
                btnStore.RemoveAt(btnStore.Count - 1);
            }
        }
        string directory = ECFile.DirectoryName("../Resources/Image/");
        string[] files = ECFile.FileCounts(directory, "png");
        //Debug.Log(String.Join(", ", files));
        List<string> names = new List<string>();
        for (int i = 0; i < files.Length; i++) names.Add(ECFile.FileName(files[i]).ToUpper());
        for (int i = 0; i < numOfPrograms; i++)
        {
            Transform t = btnStore[i].transform;
            t.GetChild(1).GetComponent<Text>().text = programs[i].name;
            t.name = "" + i;
            //Debug.Log(programs[i].icon);
            if (names.Contains(programs[i].icon.ToUpper()))
            {
                t.GetChild(2).GetComponent<RawImage>().texture = ECFile.LoadImage(directory + ECFile.FileName(files[names.IndexOf(programs[i].icon.ToUpper())]) + ".png");
            }
            else
            {
                t.GetChild(2).GetComponent<RawImage>().texture = (Texture2D)Resources.Load("Image/defaultApp");
            }
        }
        btnWidth = 450 * (numOfPrograms - 1);
        if (btnPos > btnWidth) btnPos = btnWidth;
    }

    public void SwapProgram()
    {
        if (ECUIEvent.releasedObject)
        {
            int f = -1;
            int.TryParse(ECUIEvent.releasedObject.transform.parent.name, out f);
            ECUIEvent.releasedObject = null;
            if (hightLight.transform.localPosition.y == 500) hightLight.transform.localPosition = new Vector3(585, -50 - f * 80, 0);
            else
            {
                int s = -(int)(hightLight.transform.localPosition.y + 50) / 80;
                hightLight.transform.localPosition = new Vector3(585, 500, 0);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if(s > f) for (int i = s; i > f; i--) SwapProgramContent(i, i - 1);
                    else for (int i = s; i < f; i++) SwapProgramContent(i, i + 1);
                }
                else SwapProgramContent(f, s);
            }
        }
    }

    void SwapProgramContent(int f, int s)
    {
        Transform t1 = programStore[f].transform;
        Transform t2 = programStore[s].transform;
        string[] tmp = new string[5];
		for (int t = 1; t <= tmp.Length; t++)
        {
            tmp[t - 1] = t1.GetChild(t).GetComponent<InputField>().text;
            t1.GetChild(t).GetComponent<InputField>().text = t2.GetChild(t).GetComponent<InputField>().text;
            t2.GetChild(t).GetComponent<InputField>().text = tmp[t - 1];
        }
    }

    public void OpenConfirm(int msg)
    {
        sfx[4].Play();
        confirmMsg = msg;
        switch (msg)
        {
            case 1: windowsMsg[0].text = "你確定要關閉程式？"; CloseEnder(); break;
            case 2: windowsMsg[0].text = "你確定要關閉系統？"; CloseEnder(); break;
            case 3: windowsMsg[0].text = "你確定要登出？"; break;
            default: windowsMsg[0].text = ""; break;
        }
        windows[0].FadeIn();
    }
    public void OpenAlert(int msg)
    {
        alertMsg = msg;
        switch (msg)
        {
            case 1: windowsMsg[1].text = "登入成功！"; sfx[2].Play(); break;
            case 2: windowsMsg[1].text = "登入失敗。"; sfx[3].Play(); break;
            case 3: windowsMsg[1].text = "404 Not Found."; sfx[3].Play();
                currentPath = ""; currentProgram = ""; startMsg = -1; break;
            default: windowsMsg[1].text = ""; sfx[4].Play(); break;
        }
        windows[1].FadeIn();
    }
    public void OpenLoad(int msg)
    {
        sfx[4].Play();
        loadMsg = msg;
        switch (msg)
        {
            case 1: 
            case 7:
                windowsMsg[2].text = "正在偵測投影機...";
                loadingContents.sizeDelta = new Vector2(36 * (8 + 3), 36);
                loadingBuffer = 3; ProjectorPart.order = "Detect"; break;
            case 2:
            case 6:
                windowsMsg[2].text = "開啟投影機...";
                loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
                loadingBuffer = 3; ProjectorPart.order = "TurnOn"; break;
            case 3:
            case 17:
                windowsMsg[2].text = "關閉投影機...";
                loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
                loadingBuffer = 1; ProjectorPart.order = "TurnOff"; break;
            case 4:
                windowsMsg[2].text = "開啟2D模式...";
                loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
                loadingBuffer = 1; ProjectorPart.order = "Set2D"; break;
            case 5:
                windowsMsg[2].text = "開啟3D模式...";
                loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
                loadingBuffer = 1; ProjectorPart.order = "Set3D"; break;
            case 8:
                windowsMsg[2].text = "更新投影機資訊...";
                loadingContents.sizeDelta = new Vector2(36 * (8 + 3), 36);
                loadingBuffer = 1; ProjectorPart.order = "Update"; break;
			case 9:
				windowsMsg [2].text = "連接投影機...";
				loadingContents.sizeDelta = new Vector2 (36 * (6 + 3), 36);
				loadingBuffer = 3;ProjectorPart.order = "Disconnect";
				StartCoroutine (WaitForDelay (3));
				break;
            case 10:
                windowsMsg[2].text = "正在開啟程式...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 1; break;
            case 11:
                windowsMsg[2].text = "正在關閉系統...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 2; ProjectorPart.order = "TurnOff"; break;
            case 12:
                windowsMsg[2].text = needLogin ? "取消登錄需求..." : "啟動登錄需求...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 1; break;
            case 13:
                windowsMsg[2].text = currentAutoShut ? "取消自動關機..." : "啟動自動關機...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 1; break;
            case 14:
                windowsMsg[2].text = "更新登錄資訊...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 1; break;
            case 15:
                windowsMsg[2].text = "正在關閉程式...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 2; break;
            case 16:
                windowsMsg[2].text = "開啟檢測程式...";
                loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
                loadingBuffer = 6; ProjectorPart.order = "TurnOff"; break;
			case 18:
				float posX = windowsMsg [2].rectTransform.position.x;
				posX = 240f;
				windowsMsg [2].text = "開啟3D DualHead 模式...";
				windowsMsg [2].alignment = TextAnchor.MiddleLeft;
				loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
				loadingBuffer = 1; ProjectorPart.order = "Set3DDualHead"; break;
			case 19:
				windowsMsg [2].text = "變更低清模式...";
				loadingContents.sizeDelta = new Vector2 (36 * (7 + 3), 36);
				loadingBuffer = 4;ProjectorPart.order = "Set3D";StartCoroutine (WaitForSeconds(1)); ProjectorPart.order = "HDMI"; break;
			case 20:
				windowsMsg[2].text = "變更高清模式...";
				loadingContents.sizeDelta = new Vector2(36 * (7 + 3), 36);
				loadingBuffer = 4; ProjectorPart.order = "Set3DDualHead"; StartCoroutine (WaitForSeconds(1));ProjectorPart.order = "DPort"; break;
			default: windowsMsg[2].text = ""; break;
        }
        windows[2].FadeIn();
    }

	// For Advanced Projector Setting, Pop up loading windows
	public void OpenLoadProjector(int msg)
	{
		sfx[4].Play();
		loadMsg = msg;
		switch (msg)
		{
		case 5: 
			windowsMsg[2].text = "開啟投影機...";
			loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
			loadingBuffer = 3; break;
		case 2:
			windowsMsg[2].text = "關閉投影機...";
			loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
			loadingBuffer = 1; break;
		case 3:
			windowsMsg[2].text = "開啟2D模式...";
			loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
			loadingBuffer = 1; break;
		case 4:
			windowsMsg[2].text = "開啟3D模式...";
			loadingContents.sizeDelta = new Vector2(36 * (6 + 3), 36);
			loadingBuffer = 1; break;
		default: windowsMsg[2].text = ""; break;
		}
		windows[2].FadeIn();
	}


    public void CloseConfirm(bool run)
    {
        windows[0].FadeOut();
        if (run)
        {
            switch (confirmMsg)
            {
                case 1: ShutDown(15); break;
                case 2: ShutDown(11); break;
                case 3: QuitProgram(); OpenLoad(17); break;
                default: break;
            }
        }
        confirmMsg = 0;
    }
    public void CloseAlert()
    {
        windows[1].FadeOut();
        switch (alertMsg)
        {
            case 1:
                if (numOfProjectors > 0 && isIni) { ChangePage(1); }
                else ChangePage(2);
                break;
            case 2: break;
            default: break;
        }
        alertMsg = 0;
    }
    public void CloseLoad()
    {
        windows[2].FadeOut();
        switch (loadMsg)
        {
            case 1: StartCoroutine(LoadMsg(2, 0.5f)); break;
            case 2:
                if (!needLogin || isIni) ChangePage(2);
                else ChangePage(0);
                break;
            case 10: ChangePage(3); break;
            case 11: isQuit = true; ProjectorPart.order = "ShutDown"; break;
            case 12: LoginRequirement(); break;
            case 13: AutoShut(); break;
            case 14: LoginInfoChange(); break;
            case 15: isQuit = true; ProjectorPart.order = "Quit"; break;
            case 17: ChangePage(0); break;
            default: break;
        }
        loadMsg = 0;
    }

    public void OpenStarter(int program = -1)
    {
        int msg = -1;
        if (program >= 0) msg = program;
        else if (ECUIEvent.releasedObject)
        {
            int.TryParse(ECUIEvent.releasedObject.name, out msg);
            ECUIEvent.releasedObject = null;
        }
        if (msg >= 0)
        {
            selectedProgram = msg;
            btnPos = selectedProgram * 450;
            StartCoroutine(StarterDelay(msg));
        }
    }
    IEnumerator StarterDelay(int msg)
    {
        startMsg = msg;
        List<string> tmp = new List<string>();
        tmp.AddRange(ECFile.FileCounts(ECFile.DirectoryName(programs[msg].path), "bat"));
        List<string> files = new List<string>();
        foreach (string s in tmp) if (ECFile.FileName(s.ToUpper()).Contains(programs[msg].type.ToUpper())) files.Add(s);
        //Debug.Log(String.Join(", ", files.ToArray()));
        if (files.Count > 0)
        {
            //switch (programs[msg].type.ToUpper())
            //{
            //    case "PC": System.Diagnostics.Process.Start("cmd", "/C cd /D " + ECFile.Path("../Check") + " & start /min " + ECFile.Path("../Check/CheckVRDeviceScene_PC.bat")); break;
            //    default: System.Diagnostics.Process.Start("cmd", "/C cd /D " + ECFile.Path("../Check") + " & start /min " + ECFile.Path("../CheckCheckVRDeviceScene_CAVE.bat")); break;
            //}
            //OpenLoad(16);
            //do { yield return 0; } while (loadMsg > 0);
            sfx[4].Play();
            currentProgram = files[files.Count - 1];
            int set = 0;
            switch (programs[msg].name.ToUpper())
            {
                case "DETECTIVEBOULEVARD":
                case "DETECTIVE BOULEVARD":
                case "PATCHUP": set = 1; break;
                case "TROVE": set = 2; break;
                case "SURVIVALSHOOTER":
                case "SURVIVAL SHOOTER": set = 3; break;
                default: set = 0; break;
            }
            for (int i = 0; i < startSet.Length; i++) startSet[i].SetActive(i == set);
            windows[3].FadeIn();
        }
        else OpenAlert(3);
        yield return 0;
    }

    public void CloseStarter(bool run)
    {
        windows[3].FadeOut();
        //System.Diagnostics.Process.Start("cmd", "/C taskkill /FI \"IMAGENAME eq CheckVRDeviceScene*\" & taskkill /FI \"IMAGENAME eq cmd*\"");
        if (run) LoadProgram(startMsg);
        startMsg = -1;
    }
    public void OpenEnder()
    {
        if (currentAutoShut && ECFile.FileExists("../Resources/ShutDown.bat")) OpenConfirm(2);
        else
        {
            sfx[4].Play();
            endMsg = 0;
            windows[7].FadeIn();
        }
    }
    public void CloseEnder()
    {
        endMsg = -1;
        windows[7].FadeOut();
    }
    public void OpenSetting(int msg)
    {
		if (settingMsg == msg) { CloseSetting(false); }
        else
        {
            sfx[4].Play();
            settingMsg = msg;
            for (int i = 1; i < 4; i++)
            {
                if (msg == i)
                {
                    switch (i)
                    {
                        case 2: tabSelector.SetTabObjects(inputP2); numOfProjectors = projectors.Length; RenewProjectors(0); break;
                        case 3: tabSelector.SetTabObjects(inputP3); numOfPrograms = programs.Length; RenewPrograms(0); break;
                    }
                    windows[3 + i].FadeIn();
                }
                else windows[3 + i].FadeOut();
            }
        }
    }
    public void CloseSetting(bool run)
    {
//		udpTesting.SetActive(false);
        windows[3 + settingMsg].FadeOut();
        if (run)
        {
            switch (settingMsg)
            {
                case 2: ConfirmRenewProjectors(); break;
                case 3: ConfirmRenewPrograms(); break;
                default: break;
            }
        }
        settingMsg = 0;
        if (page == 0) tabSelector.SetTabObjects(inputP0);
        else tabSelector.SetTabObjects(inputP1);
    }

    public void Login()
    {
        if (ac == inputP0[0].GetComponent<InputField>().text && pw == inputP0[1].GetComponent<InputField>().text) OpenAlert(1);
        else OpenAlert(2);
    }
    public void Logout()
    {
        OpenConfirm(3);
    }

    public void ChangePage(int current)
    {
        switch(current)
        {
            case 0:
                inputP0[0].GetComponent<InputField>().text = "";
                inputP0[1].GetComponent<InputField>().text = "";
                break;
            case 1: StartCoroutine(LoadMsg(1, 0.5f)); break;
            case 2: isIni = true; break;
        }
        pages[page].FadeOut();
        page = current;
        pages[page].FadeIn();
        tabSelector.SetTabObjects(current == 0 ? inputP0 : inputP1);
    }
    
    public void ShowSlide(int value)
    {
        selectedProgram += value;
        btnPos = selectedProgram * 450;
    }

    public void WindowMode(float delay)
    {
        StartCoroutine(SetWindowMode(delay));
    }

    IEnumerator SetWindowMode(float delay)
    {
        Screen.SetResolution(Screen.width, Screen.height, true);
        float timer = 0;
        do { timer += Time.deltaTime; yield return 0; } while (timer < delay);        
        currentFullScreen = !currentFullScreen;
        if (currentFullScreen) ECCommons.SetMonitorIndex(sMon, false);
        else ECCommons.SetMonitorIndex(sMon, false, sWidth, sHeight);
        setBtns[0].text = currentFullScreen ? "關閉全螢幕" : "開啟全螢幕";
        PlayerPrefs.SetInt("Full", currentFullScreen ? 1 : 0);
    }

    public void AutoShut()
    {
        currentAutoShut = !currentAutoShut;
        PlayerPrefs.SetInt("Auto", currentAutoShut ? 1 : 0);
    }
    public void LoginRequirement()
    {
        if (page == 0 && needLogin)
        {
            if (numOfProjectors > 0 && isIni) { ChangePage(1); }
            else ChangePage(2);
        }
        needLogin = !needLogin;
        PlayerPrefs.SetInt("Login", needLogin ? 1 : 0);
    }

    public void LoginInfoChange()
    {
        ac = inputP0[0].GetComponent<InputField>().text;
        pw = inputP0[1].GetComponent<InputField>().text;
        PlayerPrefs.SetString("AC", ac);
        PlayerPrefs.SetString("PW", pw);
    }

    IEnumerator LoadMsg(int msg, float delay)
    {
        float timer = 0;
        while(timer < delay)
        {
            yield return 0;
            timer += Time.deltaTime;
        }
        OpenLoad(msg);
    }

    void LoadProgram(int index)
    {
        StartCoroutine(LoadProgramDelay(index));
    }

    IEnumerator LoadProgramDelay(int msg)
    {
        OpenLoad(10);
        ChangeEnvironment(programs[msg].mvrVersion.Length > 0 ? programs[msg].mvrVersion : defaultMVRVer);
        float timer = 0;
        while (timer < 2) { timer += Time.deltaTime; yield return 0; }
        string detectFile = "../Resources/Detecter.bat";
        string content = File.ReadAllLines(currentProgram)[0];
        string path = ECFile.DirectoryName(currentProgram).Replace("\\", "/");
        currentPath = content.Substring(1, content.IndexOf(".exe") - 1);
        List<string> detecter = new List<string>();
        detecter.Add("@echo off");
        detecter.Add("timeout /t 3 /nobreak > NUL");
        detecter.Add("cd /D C:\\Windows\\System32");
        detecter.Add(":CheckPoint");
        detecter.Add("timeout /t 1 /nobreak > NUL");
        detecter.Add("@tasklist|find /i /c \"" + currentPath + ".exe\" > NUL");
        detecter.Add("@IF %ERRORLEVEL% EQU 0 @GOTO CheckPoint");
        detecter.Add("MD " + ECFile.DirectoryName(detectFile) + "\\AppClosed");
        detecter.Add("taskkill /FI \"IMAGENAME eq cmd*\"");
        File.WriteAllLines(detectFile, detecter.ToArray());
        System.Diagnostics.Process.Start("cmd", "/C cd /D " + path + " & start /min " + ECFile.Path(currentProgram) + " & start /min " + ECFile.Path(detectFile) + "");
        isRun = true;
    }

	IEnumerator WaitForDelay(int seconds)
	{
		yield return new WaitForSeconds (seconds);
		Debug.Log ("Ports have disconnected.");
		windowsMsg[2].text = "設定投影機...";
		loadingBuffer = 1; ProjectorPart.order = "Connect";
		windows[2].FadeIn();
		yield return new WaitForSeconds (1);
		windows [2].FadeOut ();
	}

	IEnumerator WaitForSeconds(int seconds)
	{
		yield return new WaitForSeconds (seconds);
	}

    public void QuitProgram()
    {
        if (currentPath != "")
        {
            System.Diagnostics.Process.Start("cmd", "/K cd /D C:\\Windows\\System32 & taskkill /FI \"IMAGENAME eq " + currentPath + ".exe\" & taskkill /FI \"IMAGENAME eq cmd*\"");
            currentPath = "";
            isRun = false;
        }
    }

    public void ChangeEnvironment(string value)
    {
        if (currentEnvironment != value && environment.ContainsKey(value))
        {
            currentEnvironment = value;
            Environment.SetEnvironmentVariable("Path", environment[currentEnvironment]);
            //System.Diagnostics.Process.Start("cmd", "/C start " + ECFile.Path("../Resources/Environment/" + currentEnvironment + ".bat"));
        }
    }

    IEnumerator FadeOut()
    {
        float timer = 1;
        while (timer > 0)
        {
            yield return 0;
            timer -= Time.deltaTime;
        }
        timer = 1;
        while (timer > 0)
        {
            yield return 0;
            timer -= Time.deltaTime;
            black.color = new Color(0, 0, 0, timer);
        }
        Destroy(black);
    }
    void ConfirmShutDown()
    {
        OpenConfirm(3);
    }

    void ShutDown(int msg)
    {
        QuitProgram();
        OpenLoad(msg);
    }

    void OnApplicationQuit()
    {
        if (!isQuit)
        {
            Application.CancelQuit();
            if (confirmMsg <= 0 && loadMsg <= 0 && alertMsg <= 0 && startMsg < 0 && endMsg < 0) OpenEnder();
        }
    }
}
