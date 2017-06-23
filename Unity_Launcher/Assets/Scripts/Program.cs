using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Program
{
    public string name;
    public string path;
    public string type;
    public string mvrVersion;
    public string icon;

    public void Set(int index, string value)
    {
        switch (index)
        {
            case 1: path = value; break;
            case 2: type = value; break;
            case 3: mvrVersion = value; break;
            case 4: icon = value; break;
            default: name = value; break;
        }
    }
}
