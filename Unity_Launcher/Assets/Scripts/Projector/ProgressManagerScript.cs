using System.Collections;
using System.Collections.Generic;
using SystemD = System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManagerScript : MonoBehaviour {

    public Image progressImage;

    private static Vector3 PROGRESS_STEP = new Vector3(0, 0, 360 / 8);
    private const int PROGRESS_FRAME_SKIP = 5;

    private static ProgressManagerScript instance;
    private List<string> jobList;

    public bool isAnimating {
        get {
            return jobList.Count != 0;
        }
    }

    // Use this for initialization
    void Start () {
        jobList = new List<string>();
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
		if (isAnimating) {
            progressImage.gameObject.SetActive(true);
            if (Time.frameCount % PROGRESS_FRAME_SKIP == 0) {
                progressImage.transform.localRotation = Quaternion.Euler(progressImage.transform.localRotation.eulerAngles + PROGRESS_STEP);
            }
        } else {
            progressImage.gameObject.SetActive(false);
        }
	}

    public static void DoJob (string name) {
        if (instance != null) {
            instance.jobList.Add(name);
            Debug.Log(name + " has added.");
        }
    }

    public static void FinishJob (string name) {
        if (instance != null) {
            instance.jobList.Remove(name);
            Debug.Log(name + " has removed.");
        }
    }
}
