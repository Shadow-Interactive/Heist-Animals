using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using XBOX;

public class RadialButton : MonoBehaviour {

    public int buttonCamVal;
    public Vector2 rotationSelectVal;
    public OverSeerControl overseer;
    public Button buttonCam;
    public EventSystem radialSystem;
    bool select = false;

	// Use this for initialization
	void Start () {
        
        buttonCam.onClick.AddListener(switchToCam);
	}

    void switchToCam()
    {
        overseer.radCamSelect = buttonCamVal;
        overseer.SetCam(buttonCamVal);
    }

    void Update()
    {
        print(XBoxInput.GetRightX());
        //want to gather the overseer joystick axis and have that value determine which radial button is selected
        if (XBoxInput.GetRightY() <= rotationSelectVal.y + 0.15 && XBoxInput.GetRightY() >= rotationSelectVal.y - 0.15 && XBoxInput.GetRightX() <= rotationSelectVal.x + 0.15 && XBoxInput.GetRightX() >= rotationSelectVal.x - 0.15)
        {
            select = true;
        }
        else
            select = false;

        if (select == true)
        {
            radialSystem.SetSelectedGameObject(gameObject);
        }
    }
}
