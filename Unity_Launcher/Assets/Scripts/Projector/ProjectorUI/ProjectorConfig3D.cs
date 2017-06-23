using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectorConfig3D : MonoBehaviour {

	CustomProjectorScript customProjectorScript;
	public Button btn_3DDH;
	public InputField configPath;

	void Start()
	{
		//CheckConfigPath_Barco();
	}

	// Advance Setting (Alt-Shift-2): check each projector, is it a BARCO projector or not
	public void CheckConfigPath_Barco()
	{
		// BARCO Projector
		if (configPath.text.Contains ("barco")) {
			btn_3DDH.interactable = true; // Enable the 3D(DH) button
			btn_3DDH.transform.GetChild (1).GetComponent<Text> ().color = new Color (255, 255, 255, 255); // Text with white color
		}
		// Other Projectors
		else {
			btn_3DDH.interactable = false; // Disenable the 3D(DH) button
			btn_3DDH.transform.GetChild(1).GetComponent<Text>().color = new Color(0.75f, 0.75f, 0.75f, 1f); // Grey out the text color
		}
	}

	// Setting (Alt-Shift-1) : check if no BARCO projector exists, Disable the BATCH 3D(DH) button  
	public void CheckProjectors_Barco(Button btn_batch3DDH)
	{
		CustomProjectorScript[] customProjectorObjects = (CustomProjectorScript[])GameObject.FindObjectsOfType (typeof(CustomProjectorScript));
		foreach (CustomProjectorScript customProjectorObject in customProjectorObjects) {
			if (customProjectorObject.GetComponent<CustomProjectorScript> ().inputConfigPath.text.Contains ("barco")) {
				btn_batch3DDH.interactable = true;
				btn_batch3DDH.transform.GetChild (1).GetComponent<Text> ().color = new Color (255, 255, 255, 255); // Text with white color
				break;
			} else {
				btn_batch3DDH.interactable = false;
				btn_batch3DDH.transform.GetChild (1).GetComponent<Text> ().color = new Color (0.75f, 0.75f, 0.75f, 1f); // Grey out the text color
		
			}
		}
	}
}
