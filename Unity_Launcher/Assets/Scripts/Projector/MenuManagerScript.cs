using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuManagerScript : MonoBehaviour, IProjectManagerScript  {

    public List<GameObject> menus;
    public int defaultMenuIndex;

	// Use this for initialization
	void Start () {
        foreach (GameObject m in menus) {
            m.SetActive(true);
        }
        GoToMenu(defaultMenuIndex);

        ProjectorManagerScript.RegisterListener(this);
    }
	
	// Update is called once per frame
	void Update () {
        // hold down space
        if (true) {
            if (Input.GetKeyDown(KeyCode.F2)) {
                GoToMenu(1);
            } else if (Input.GetKeyDown(KeyCode.F3)) {
                GoToMenu(2);
            } else if (Input.GetKeyDown(KeyCode.F1)) {
                GoToMenu(0);
            }
        }	    
	}

    private void SetMenusActive (bool isActive, int except = -1) {
        for (int i = 0; i < menus.Count; i++) {
            GameObject m = menus[i];
            //m.SetActive(isActive);
            //m.GetComponent<MeshRenderer>().enabled = isActive;
            m.transform.localScale = isActive ? Vector3.one : Vector3.zero;
        }

        if (except >= 0) {
            menus[except].transform.localScale = Vector3.one;
        }
    }

    public void GoToMenu (int index) {
        SetMenusActive(false, index);
        Debug.Log("show: " + index);
    }

    public void OnDetectProjectors(string[] portNames) {
        Debug.Log("OnDetect: " + string.Join(", ", portNames));
    }

    public void OnLoadDefaultProjectors(string[] portNames) {
        Debug.Log("OnLoad: " + string.Join(", ", portNames));
    }
}
