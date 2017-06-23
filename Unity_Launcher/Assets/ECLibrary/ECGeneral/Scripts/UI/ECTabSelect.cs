using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ECTabSelect : MonoBehaviour {
    private EventSystem eventSystem;
    public KeyCode tabKey = KeyCode.Tab;
    public bool isTab = false;
    public int currentID = 0;

    public List<GameObject> tabObject = new List<GameObject>();
    List<Selectable> tabList = new List<Selectable>();
    // Use this for initialization
    void Start () {
        this.eventSystem = EventSystem.current;
        if(tabObject.Count > 0) SetTabObjects(tabObject);
    }
	
	// Update is called once per frame
	void Update () {
        if ((isTab || Input.GetKeyDown(tabKey)) && tabList.Count > 0)
        {
            isTab = false;
            currentID = (tabObject.IndexOf(ECUIEvent.selectedObject) + 1) % tabList.Count;
            tabList[currentID].Select();
        }
    }

    public void GetCurrent()
    {
        currentID = Mathf.Max(0, Mathf.Min(currentID, tabList.Count - 1));
        tabList[currentID].Select();
    }

    public void SetTabObjects(Selectable[] selectables, int selected = -1)
    {
        List<Selectable> tmp = new List<Selectable>();
        tmp.AddRange(selectables);
        tabList.Clear();
        tabList.AddRange(selectables);
        tabObject.Clear();
        foreach (Selectable s in tabList) tabObject.Add(s.gameObject);
        AddUIEvent();
        if (selected >= 0) GetCurrent();
        else currentID = -1;
    }
    public void SetTabObjects(GameObject[] gameObjects, int selected = -1)
    {
        List<GameObject> tmp = new List<GameObject>();
        tmp.AddRange(gameObjects);
        tabObject.Clear();
        tabObject.AddRange(tmp);
        tabList.Clear();
        foreach (GameObject g in tabObject) tabList.Add(g.GetComponent<Selectable>());
        AddUIEvent();
        if (selected >= 0) GetCurrent();
        else currentID = -1;
    }
    public void SetTabObjects(List<Selectable> selectables, int selected = -1)
    {
        SetTabObjects(selectables.ToArray(), selected);
    }
    public void SetTabObjects(List<GameObject> gameObjects, int selected = -1)
    {
        SetTabObjects(gameObjects.ToArray(), selected);
    }

    void AddUIEvent()
    {
        for(int i = 0; i < tabObject.Count; i++)
        {
            if(!tabObject[i].GetComponent<ECUIEvent>())
            {
                tabObject[i].AddComponent<ECUIEvent>();
            }
        }
    }
}
