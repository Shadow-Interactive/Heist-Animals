using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamUpdate : MonoBehaviour {

    //this class is used for the camera in LobbyScene
    public float rotateSpeed;

	// Use this for initialization
	void Start () {
        rotateSpeed = 7.5f;
	}
	
	// Update is called once per frame
	void Update () {

        //just want it to rotate around y-axis
        gameObject.transform.Rotate(new Vector3(0, Time.deltaTime * rotateSpeed, 0), Space.World);
	}
}
