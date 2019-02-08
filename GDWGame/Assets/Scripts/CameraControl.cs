using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XBOX;

public class CameraControl : MonoBehaviour {

    public GameObject player;
    public Vector2 cameraLook;

	// Use this for initialization
	void Start () {

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        
        if(XBoxInput.GetConnected())
        {
            cameraLook.x += XBoxInput.GetRightX();
            cameraLook.y += XBoxInput.GetRightY();
        }
        else
        {
            cameraLook.x += Input.GetAxis("Mouse X");
            cameraLook.y += Input.GetAxis("Mouse Y");
        }

        cameraLook.y = Mathf.Clamp(cameraLook.y, -50.0f, 50.0f);
;

        Quaternion cameraRotate = Quaternion.Euler(-cameraLook.y, cameraLook.x, 0.0f);
        transform.rotation = cameraRotate;
    }

    void LateUpdate()
    {
        camFollow();
    }

    void camFollow()
    {
        Transform target = player.transform;
        transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0, 1, 0), 120 * Time.deltaTime);
    }


}
