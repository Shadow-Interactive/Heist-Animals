using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour {
    //http://wiki.unity3d.com/index.php/CameraFacingBillboard
    public Camera theCamera;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        if (theCamera != null)
        {
            transform.LookAt(transform.position + theCamera.transform.rotation * Vector3.forward,
              theCamera.transform.rotation * Vector3.up);
        }
    }
}
