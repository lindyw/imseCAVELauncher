using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Output : MonoBehaviour
{
    public int oriDigit = 2;
    public int tarDigit = 104;
    public Text str;
    public string value = "string";
    AudioSource a;
    public string result;

	// Use this for initialization
	void Start () {
        //Debug.Log(Files.FilesCounted("Video", "", "avi").Length);
        //Debug.Log(FileIO.Path("A/B/C/D.png"));
        //a.clip = FileIO.LoadAudio("../../Piano/A4");
        //StartCoroutine(FileIO.LoadWAV(a = gameObject.AddComponent<AudioSource>(), "Piano/A4.wav"));
        //str.text = ECCommons.BaseConvert(value, oriDigit, tarDigit, ECConstant.B128);
        //Debug.Log("ABC".IndexOf('A'));
        string s = ECConstant.B256;
        value = s[72].ToString();
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
            //StartCoroutine(FileIO.LoadWAV(a = gameObject.AddComponent<AudioSource>(), "Piano/A4.wav"));
            //a = ECCommons.SetComponent<AudioSource>(gameObject);
            //a.clip = FileIO.LoadAudio("Piano/A4.wav");
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) a.Play();
        str.text = ECCommons.BaseConvert(value, oriDigit, tarDigit, ECConstant.B256);
        result = str.text;
	}
}
