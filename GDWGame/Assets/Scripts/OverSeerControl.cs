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

    //can optimize later
    public GameObject cam1, cam2, cam3, cam4, cam5, cam6, cam7, cam8, cam9, cam10, cam11, cam12;

    private List<GameObject> totalCamera = new List<GameObject>();

    private Vector2 cameraLook;
    private List<Vector3> ogRotation = new List<Vector3>();

    public bool camChoice = true;
    bool initRM = false;
    
    string mouseX = "Mouse X", mouseY = "Mouse Y";
    string treasureStr = "Treasure", securityStr = "SecurityBox";

    RaycastHit clickHit;
    Ray clickRay;

    RoomManager theRoomManager;
    
    Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);
    public Image cursor;

    int currentCamera = 0;

    public OverseerCanvasManager theCanvasManager;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        gameObject.transform.position = new Vector3(9.5f, -1.5f, 1.1f);
        //cam1.transform.position = new Vector3(-12.591f, 3.0f, -4.65f);
        //cam2.transform.position = new Vector3(-17.47f, 3.0f, 17.7f);

        //load in our cameras
        totalCamera.Add(cam1);
        totalCamera.Add(cam2);
        totalCamera.Add(cam3);
        totalCamera.Add(cam4);
        totalCamera.Add(cam5);
        totalCamera.Add(cam6);
        totalCamera.Add(cam7);
        totalCamera.Add(cam8);
        totalCamera.Add(cam9);
        totalCamera.Add(cam10);
        totalCamera.Add(cam11);
        totalCamera.Add(cam12);
        //totalCamera.Add(cam13);
        
        //original rotations to reset to
        ogRotation.Add(cam1.transform.rotation.eulerAngles);
        ogRotation.Add(cam2.transform.rotation.eulerAngles);
        ogRotation.Add(cam3.transform.rotation.eulerAngles);
        ogRotation.Add(cam4.transform.rotation.eulerAngles);
        ogRotation.Add(cam5.transform.rotation.eulerAngles);
        ogRotation.Add(cam6.transform.rotation.eulerAngles);
        ogRotation.Add(cam7.transform.rotation.eulerAngles);
        ogRotation.Add(cam8.transform.rotation.eulerAngles);
        ogRotation.Add(cam9.transform.rotation.eulerAngles);
        ogRotation.Add(cam10.transform.rotation.eulerAngles);
        ogRotation.Add(cam11.transform.rotation.eulerAngles);
        ogRotation.Add(cam12.transform.rotation.eulerAngles);
        //ogRotation.Add(cam13.transform.rotation.eulerAngles);
        

        if (camChoice)
            totalCamera[0].GetComponentInChildren<Camera>().enabled = true;
        else
            totalCamera[0].GetComponentInChildren<Camera>().enabled = false;

        for (int i = 1; i < totalCamera.Count; i++)
        {
            totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
        }

       theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
    }

    void findFOV(int x)
    {
        test = totalCamera[x].GetComponentInChildren<Camera>().fieldOfView;
        return;
    }

    public void LoadProperties()
    {
        currentCamera = 0;
        gameObject.transform.position = new Vector3(9.5f, -1.5f, 1.1f);

        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
       
        //cursor.GetComponent<RectTransform>().anchoredPosition = viewportCenter;
        theCanvasManager.LoadProperties(theRoomManager);

        initRM = true;
    }


	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            for (int i = 0; i < totalCamera.Count; i++)
            totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            camChoice = false;
            theCanvasManager.SetCanvasActive(false);
            return;
        }

        if (!initRM)
        {
            LoadProperties();
        }

        cameraLook.x += Input.GetAxis("Mouse X");
        cameraLook.y += Input.GetAxis("Mouse Y");

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);
        cameraLook.x = Mathf.Clamp(cameraLook.x, CAM_X_MIN, CAM_X_MAX);
        //Debug.Log(totalCamera.Count);
        //array will be way more useful for this stuff later on
        
        //https://answers.unity.com/questions/615771/how-to-check-if-click-mouse-on-object.html
        //to activate trap
        if (Input.GetMouseButtonDown(0))
        {
            clickRay = totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter);
            //print(clickRay + " other ver " + totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter));

            if (Physics.Raycast(clickRay, out clickHit))
            {
                if (clickHit.transform.CompareTag(treasureStr))
                {
                    //atm, the overseer can only have 5 active traps at a time
                    clickHit.collider.gameObject.GetComponent<Treasure>().TreasureOnClick(theRoomManager.theImages,gameObject.GetComponent<OverSeerControl>());//, this.gameObject.GetComponent<OverSeerControl>());
                }
                else if (clickHit.transform.CompareTag(securityStr))
                {
                    clickHit.collider.gameObject.GetComponent<TrapDoor>().OnSecurityClick();
                }
            }
           
        }

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
                        currentCamera = i - 1;
                        theCanvasManager.SwitchCameras(currentCamera);
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        break;
                    }
                    //totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                }
                theCanvasManager.UpdateCanvasCamera(totalCamera[i].GetComponentInChildren<Camera>());
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
                        currentCamera = i + 1;
                        theCanvasManager.SwitchCameras(currentCamera);
                        break;
                    }
                }
                theCanvasManager.UpdateCanvasCamera(totalCamera[i].GetComponentInChildren<Camera>());

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

        
    }

    public void roleChose(OverSeerControl script)
    {
        script.camChoice = !script.camChoice;
        //script2.spawnIn = false;
        this.camChoice = script.camChoice;
        Debug.Log(camChoice);
    }

    public int GetNumTrap()
    {
        return theCanvasManager.numTrapActivated;
    }

    public void ObjectiveActivate(ref int currentObjectID, int ID)
    {
        theCanvasManager.numTrapActivated++;
        currentObjectID = theCanvasManager.currentTrap;
        theCanvasManager.SetTrapIconActive(currentObjectID, Color.green, ID);
        theCanvasManager.SetCurrentlyActive();        
    }

    public void DecoupleTrap(int currentObjectID, Color theColor, int ID)
    {
        theCanvasManager.numTrapActivated--;
        theCanvasManager.SetTrapIconActive(currentObjectID, theColor, ID);
        theCanvasManager.SetCurrentlyActive();
    }
}
