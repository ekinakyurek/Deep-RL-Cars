using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {

	public Camera firstcamera;
	public Camera secondcamera;

	// Use this for initialization
	void Start () {
		firstcamera.enabled = true;
		secondcamera.enabled = false;
	}


	void OnGUI() {
		if (GUI.Button (new Rect (170, 150, 50, 50), "Switch")) {
			firstcamera.enabled = !firstcamera.enabled;
			secondcamera.enabled = !secondcamera.enabled;
		}
	}	

}
