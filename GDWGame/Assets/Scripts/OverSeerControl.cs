using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;


public class OverSeerControl : NetworkBehaviour {

    private const float CAM_Y_MIN = -80.0f;
    private const float CAM_Y_MAX = 20.0f;
    private const float CAM_X_MIN = -40.0f;
    private const float CAM_X_MAX = 40.0f;

    public float test;

    private float zoomSpeed = 20.0f;

    public GameObject cam1;
    public GameObject cam2;

    private List<GameObject> totalCamera = new List<GameObject>();

    private Vector2 cameraLook;
    private List<Vector3> ogRotation = new List<Vector3>();

    public bool camChoice = true;

    //for the trap
    public GameObject screenBlocker;
    bool EMP = false;
    float empTimer = 0;

    //all the juicy ui stuff
    public Canvas trapCanvas, regularCanvas;
    RoomManager theRoomManager;
    private List<Text> trapTexts = new List<Text>();
    private List<GameObject> trapImages = new List<GameObject>();
    GameObject txtPrefab;
    GameObject shockImage, empImage;
    GameObject imageTemp;
    GameObject[] theImages = new GameObject[4];
    private float imageHeight;

    string mouseX = "Mouse X", mouseY = "Mouse Y";

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        cam1.transform.position = new Vector3(-12.591f, 3.0f, -4.65f);
        cam2.transform.position = new Vector3(-17.47f, 3.0f, 17.7f);

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

        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        txtPrefab = Resources.Load("Test") as GameObject;
        shockImage = Resources.Load("ShockUI") as GameObject;
        empImage = Resources.Load("EMPUI") as GameObject;

        //we need images or smth to dictate no traps/safeguards
        theImages[0] = shockImage;
        theImages[1] = shockImage;
        theImages[2] = empImage;
        theImages[3] = empImage;

        for (int i = 0; i < theRoomManager.ObjectiveLength(); i++)
        {
            AddTrap(theRoomManager.GetObjPos(i), theRoomManager.GetObjCode(i));
        }

        for (int i = 0; i < theRoomManager.SecurityLength(); i++)
        {
            AddImage(theRoomManager.GetSecurityPos(i), theRoomManager.GetSecurityRotation(i), theRoomManager.getRoomTrap(i));
        }
    }

    void findFOV(int x)
    {
        test = totalCamera[x].GetComponentInChildren<Camera>().fieldOfView;
        return;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (!isLocalPlayer)
        {
            totalCamera[0].GetComponentInChildren<Camera>().enabled = false;
            camChoice = false;
            regularCanvas.gameObject.SetActive(false);
            trapCanvas.gameObject.SetActive(false);
            return;
        }
        
        cameraLook.x += Input.GetAxis("Mouse X");
        cameraLook.y += Input.GetAxis("Mouse Y");

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);
        cameraLook.x = Mathf.Clamp(cameraLook.x, CAM_X_MIN, CAM_X_MAX);
        //Debug.Log(totalCamera.Count);
        //array will be way more useful for this stuff later on

        for (int i = 0; i < totalCamera.Count; i++)
        {
            findFOV(i);

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
                trapCanvas.worldCamera = totalCamera[i].GetComponentInChildren<Camera>();
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
                trapCanvas.worldCamera = totalCamera[i].GetComponentInChildren<Camera>();
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                    if(temp > 30)
                        temp -= zoomSpeed * Time.deltaTime;

                    totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                    if (temp < 100)
                        temp += zoomSpeed * Time.deltaTime;

                    totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                }
            }

            if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
            {
                totalCamera[i].transform.eulerAngles = new Vector3(-cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
            }
        }

        if (EMP)
        {
            empTimer += Time.deltaTime;

            if (empTimer > 10)
            {
                empTimer = 0;
                EMP = false;
                screenBlocker.SetActive(EMP);
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

    public void setEMP(bool activeEMP)
    {
        EMP = activeEMP;
        screenBlocker.SetActive(activeEMP);
    }

    public bool getEMP()
    {
        return EMP;
    }

    void AddTrap(Vector3 trapPos, SyncListInt theCode)
    {
        Text newObjTxt = GameObject.Instantiate(txtPrefab.GetComponent<Text>());
        newObjTxt.transform.parent = trapCanvas.transform;
        newObjTxt.transform.position = new Vector3(trapPos.x, 2, trapPos.z);

        newObjTxt.text = (theCode[0] + " " + theCode[1] + " " + theCode[2] + " " + theCode[3]).ToString();
        trapTexts.Add(newObjTxt);
    }

    void AddImage(Transform boxPos, Vector3 boxRotation, RoomTraps theType)
    {
        imageTemp = GameObject.Instantiate(theImages[(int)theType]);
        imageTemp.transform.position = new Vector3(boxPos.position.x, boxPos.position.y + 1.7f, boxPos.position.z);
        imageTemp.transform.eulerAngles = new Vector3(boxRotation.x, boxRotation.y + 90, boxRotation.z);
        imageTemp.transform.parent = trapCanvas.transform;
        trapImages.Add(imageTemp);

    }

    public void UpdateTextCode(int[] theCode, int index)
    {
        trapTexts[index].text = (theCode[0] + " " + theCode[1] + " " + theCode[2] + " " + theCode[3]).ToString();
    }
}
