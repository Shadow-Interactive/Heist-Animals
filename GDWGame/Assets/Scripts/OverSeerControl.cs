using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


public class OverSeerControl : NetworkBehaviour {

    private const float CAM_Y_MIN = -80.0f;
    private const float CAM_Y_MAX = 20.0f;
    private const float CAM_X_MIN = -40.0f;
    private const float CAM_X_MAX = 40.0f;

    public GameObject cam1;
    public GameObject cam2;

    private List<GameObject> totalCamera = new List<GameObject>();

    private Vector2 cameraLook;
    private List<Vector3> ogRotation = new List<Vector3>();

    public bool camChoice = false;
  
    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        totalCamera.Add(cam1);
        totalCamera.Add(cam2);

        ogRotation.Add(cam1.transform.rotation.eulerAngles);
        ogRotation.Add(cam2.transform.rotation.eulerAngles);

        if (camChoice)
            totalCamera[0].GetComponentInChildren<Camera>().enabled = true;
        else
            totalCamera[0].GetComponentInChildren<Camera>().enabled = false;

        for (int i = 1; i < totalCamera.Count; i++)
        {
            totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {

        
        cameraLook.x += Input.GetAxis("Mouse X");
        cameraLook.y += Input.GetAxis("Mouse Y");

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);
        cameraLook.x = Mathf.Clamp(cameraLook.x, CAM_X_MIN, CAM_X_MAX);
        //Debug.Log(totalCamera.Count);
        //array will be way more useful for this stuff later on

        for (int i = 0; i < totalCamera.Count; i++)
        {

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    //totalCamera[i].GetComponentInChildren<Camera>().enabled = false;

                    if (totalCamera[i] == totalCamera[0])
                    {
                        totalCamera[totalCamera.Count - 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        break;
                    }
                    else
                    {
                        totalCamera[i - 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        break;
                    }

                    //totalCamera[i].GetComponentInChildren<Camera>().enabled = false;

                }
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    if (totalCamera[i] == totalCamera[totalCamera.Count - 1])
                    {
                        totalCamera[0].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        break;
                    }
                    else
                    {
                        totalCamera[i + 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        break;
                    }
                }
                
            }

            if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
            {
                totalCamera[i].transform.eulerAngles = new Vector3(-cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
            }
        }
    }

    public void roleChose(OverSeerControl script)
    {
        script.camChoice = !script.camChoice;
        //script2.spawnIn = false;
        this.camChoice = script.camChoice;
        Debug.Log(camChoice);
    }
}
