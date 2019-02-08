using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RadialButton : MonoBehaviour {

    public int buttonCamVal;
    public OverSeerControl overseer;
    public Button buttonCam;

	// Use this for initialization
	void Start () {
        buttonCam.onClick.AddListener(switchToCam);
	}

    void switchToCam()
    {
        overseer.radCamSelect = buttonCamVal;
        overseer.SetCam(buttonCamVal);
    }

}
