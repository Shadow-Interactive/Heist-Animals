using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DUIScript : MonoBehaviour {
    //http://wiki.unity3d.com/index.php/CameraFacingBillboard
    public Camera theCamera;
    public GameObject transformObject;
    
	// Update is called once per frame
	void LateUpdate () {
        if (theCamera != null)
        {
            transform.LookAt(transform.position + theCamera.transform.rotation * Vector3.forward,
              theCamera.transform.rotation * Vector3.up);
        }

        if (transformObject != null) //to move the ui around the scene
        {
            transform.position = transformObject.transform.position;
        }
    }
}
