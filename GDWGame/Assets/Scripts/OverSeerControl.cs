using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

using SoundEngine;
using XBOX;
using Ardunity;

public class OverSeerControl : NetworkBehaviour {

    public ArdunityApp controller;
    public AnalogInput controllerX;
    public AnalogInput controllerY;
    public AnalogInput controllerZ;
    public DigitalInput trapButton;
    public DigitalInput leftButton;
    public DigitalInput rightButton;
    bool trapPress;
    bool leftPress;
    bool rightPress;
    bool radialPress;

    bool ohmygoodness;
    bool doNotMove;
    bool switchCameras = false;

    private const float CAM_Y_MIN = -80.0f;
    private const float CAM_Y_MAX = 5.0f;
    private const float CAM_X_MIN = -40.0f;
    private const float CAM_X_MAX = 40.0f;

    public float test;

    private float zoomSpeed = 80.0f;
    private float lookSpeed = 2.0f;
    private float zAxis;

    float timer = 0;

    //can optimize later
    public GameObject cam1, cam2, cam3, cam4, cam5, cam6, cam7, cam8, cam9, cam10, cam11, cam12, cam13, cam14, cam15, cam16, cam17, cam18;

    private List<GameObject> totalCamera = new List<GameObject>();

    private Vector2 cameraLook;
    private List<Vector3> ogRotation = new List<Vector3>();

    public bool camChoice = true;
    bool initRM = false;
    public int radCamSelect = 0;
    int trapSelect = 0;
    
    string mouseX = "Mouse X", mouseY = "Mouse Y";
    string treasureStr = "Treasure", securityStr = "SecurityBox";
    string networkTrapStr = "TheNetworkTrap", roomStr = "Room",
        O1 = "Objective", O2 = "Objective2", O3 = "Objective3", O4 = "Objective4", O5 = "Objective5", O6 = "Objective5", O7 = "Objective7", O8 = "Objective8", O9 = "Objective9";
    public string camRoomName;

    RaycastHit clickHit;
    Ray clickRay;

    RoomManager theRoomManager;
    
    Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);

    int currentCamera = 0;

	[SyncVar] public int OverID, team;
	PlayerLogic run1; //friendly
	PlayerLogic run2; //enemy
    GameObject ally, enemy;
	ObserverPattern.EventConsole eventConsole;

    //UI related variables, including UI hints
	public OverseerCanvasManager theCanvasManager;
    public GameObject radialCamSelect, radialHintUI, AbuttonHintUI;
    bool radialNeverTriggered = true;
    float radialHintTime = 0;

    [HideInInspector] public Objective R1currentObjective;
    [HideInInspector] public Objective R2currentObjective;

    public Material run2mat;
    public GameObject overPostProcess;

    int UIUpdateCounter = 0; //this is really hacky im sorryyy im rushing T_T 
    
    //for more tutorialization stuff
    bool foundPlayer = false, foundEnemy = false;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        overPostProcess = GameObject.Find("OverseerPostVolume");
        if (isLocalPlayer)
        {
            overPostProcess.SetActive(true);
            GameObject.Find("RunnerPostVolume").SetActive(false);
        }

        
        radialCamSelect.SetActive(false);
        radialPress = false;

        //gameObject.transform.position = new Vector3(9.5f, -1.5f, 1.1f);
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
        totalCamera.Add(cam13);
        totalCamera.Add(cam14);
        totalCamera.Add(cam15);
        totalCamera.Add(cam16);
        totalCamera.Add(cam17);
        totalCamera.Add(cam18);

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
        ogRotation.Add(cam13.transform.rotation.eulerAngles);
        ogRotation.Add(cam14.transform.rotation.eulerAngles);
        ogRotation.Add(cam15.transform.rotation.eulerAngles);
        ogRotation.Add(cam16.transform.rotation.eulerAngles);
        ogRotation.Add(cam17.transform.rotation.eulerAngles);
        ogRotation.Add(cam18.transform.rotation.eulerAngles);

        if (OverID == 1)
        {
            if (camChoice)
                totalCamera[9].GetComponentInChildren<Camera>().enabled = true;
            else
                totalCamera[9].GetComponentInChildren<Camera>().enabled = false;

            for (int i = 0; i < 9; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }

            for (int i = 10; i < totalCamera.Count; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }
            camRoomName = "Room9";
            trapSelect = 9;
        }
        else if (OverID == 2)
        {
            if (camChoice)
                totalCamera[13].GetComponentInChildren<Camera>().enabled = true;
            else
                totalCamera[13].GetComponentInChildren<Camera>().enabled = false;

            for (int i = 0; i < 13; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }

            for (int i = 14; i < totalCamera.Count; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }
            camRoomName = "Room12";
            trapSelect = 13;
        }

        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();

        if (OverID == 1)
            currentCamera = 9;
        else if (OverID == 2)
            currentCamera = 13;
    }

    void findFOV(int x)
    {
        test = totalCamera[x].GetComponentInChildren<Camera>().fieldOfView;
        return;
    }

    public void LoadProperties()
    {
        if (OverID == 1)
        {
            currentCamera = 9;
            theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
        }
        else if (OverID == 2)
        {
            currentCamera = 13;
            theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
        }

        gameObject.transform.position = new Vector3(46.94304f, 9.81f, -33.79776f);

        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
       
        //cursor.GetComponent<RectTransform>().anchoredPosition = viewportCenter;
        theCanvasManager.LoadProperties(theRoomManager);

        initRM = true;


        //BECAUSE IT MIGHTVE BEEN MAKING MULTIPLE STUFF
        //the dumbstuff
        try
        {
            run1 = GameObject.FindGameObjectsWithTag("Runner")[0].GetComponent<PlayerLogic>();
            run2 = GameObject.FindGameObjectsWithTag("Runner")[1].GetComponent<PlayerLogic>();
        } catch (System.IndexOutOfRangeException e)
        {
            Debug.Log("One of the runners doesn't exist (in OverseerControl)" + e.Message);
        }

        if (OverID == 1)
		{
			eventConsole = new ObserverPattern.EventConsole(run1, run2);
			//Debug.Log("event console 1 assigned");
		}
		else if (OverID == 2)
		{
			eventConsole = new ObserverPattern.EventConsole(run2, run1);
			print("event console 2 assigned");
		}
		eventConsole.init();

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
            controller = GameObject.FindGameObjectWithTag("Ardunity").GetComponent<ArdunityApp>();
            controllerX = GameObject.FindGameObjectWithTag("CamX").GetComponent<AnalogInput>();
            controllerY = GameObject.FindGameObjectWithTag("CamY").GetComponent<AnalogInput>();
            controllerZ = GameObject.FindGameObjectWithTag("Zoom").GetComponent<AnalogInput>();
            trapButton = GameObject.FindGameObjectWithTag("TrapButton").GetComponent<DigitalInput>();
            leftButton = GameObject.FindGameObjectWithTag("LeftArrow").GetComponent<DigitalInput>();
            rightButton = GameObject.FindGameObjectWithTag("RightArrow").GetComponent<DigitalInput>();
            //camRoomName = "Room1";
            //trapSelect = 1;
            LoadProperties();
            
        }

        if(theRoomManager==null)
        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();


        if (theRoomManager.updateUIPosition)
        {
            if (UIUpdateCounter > 1) //imusingthistomakesurebothoverseersupdate
                                     //thebettersolutionwouldvebeentouseaneventmanager
                                     //butimworrieditwontworkoverunetforgivemeeee
            {
                theRoomManager.updateUIPosition = false;
                UIUpdateCounter = 0;
            }
            else
            {
                UpdateAllUI();
                UIUpdateCounter++;
            }
            
        }

        //theCanvasManager.PrintCode(theRoomManager.ObjectiveLength());
        //theCanvasManager.PrintCode(theRoomManager.ObjectiveLength());

        //for deactivating traps :)
        switch (GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().trapToGoAway)
        {
            case 1:
                R1currentObjective = GameObject.Find(O1).GetComponentInChildren<Objective>();
               
                break;
            case 2:
                R1currentObjective = GameObject.Find(O2).GetComponentInChildren<Objective>();
               
                break;
            case 3:
                R1currentObjective = GameObject.Find(O3).GetComponentInChildren<Objective>();
               
                break;
            case 4:
                R1currentObjective = GameObject.Find(O4).GetComponentInChildren<Objective>();
               
                break;
            case 5:
                R1currentObjective = GameObject.Find(O5).GetComponentInChildren<Objective>();
                
                break;
            case 6:
                R1currentObjective = GameObject.Find(O6).GetComponentInChildren<Objective>();
                
                break;
            case 7:
                R1currentObjective = GameObject.Find(O7).GetComponentInChildren<Objective>();
                
                break;
            case 8:
                R1currentObjective = GameObject.Find(O8).GetComponentInChildren<Objective>();

                break;
            case 9:
                R1currentObjective = GameObject.Find(O9).GetComponentInChildren<Objective>();

                break;
            default:
                //swing and a miss
                break;
        }

        switch (GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().otherTrapToGoAway)
        {
            case 1:
                R2currentObjective = GameObject.Find(O1).GetComponentInChildren<Objective>();
                
                break;
            case 2:
                R2currentObjective = GameObject.Find(O2).GetComponentInChildren<Objective>();
                
                break;
            case 3:
                R2currentObjective = GameObject.Find(O3).GetComponentInChildren<Objective>();
                
                break;
            case 4:
                R2currentObjective = GameObject.Find(O4).GetComponentInChildren<Objective>();
                
                break;
            case 5:
                R2currentObjective = GameObject.Find(O5).GetComponentInChildren<Objective>();
                
                break;
            case 6:
                R2currentObjective = GameObject.Find(O6).GetComponentInChildren<Objective>();
                
                break;
            case 7:
                R2currentObjective = GameObject.Find(O7).GetComponentInChildren<Objective>();
                
                break;
            case 8:
                R2currentObjective = GameObject.Find(O8).GetComponentInChildren<Objective>();

                break;
            case 9:
                R2currentObjective = GameObject.Find(O9).GetComponentInChildren<Objective>();

                break;
            default:
                //swing and a miss
                break;
        }

        
        if (controller.connected)
        {
            if (controllerX.Value < .49f || controllerX.Value > .51f)
                cameraLook.x = (controllerX.Value - .5f) * 2f;
            else
                cameraLook.x = 0;
        
            if (controllerY.Value < .48f || controllerY.Value > .52f)
                cameraLook.y = (controllerY.Value - .5f) * 2f;
            else
                cameraLook.y = 0;
        
            if (controllerZ.Value < .48f || controllerZ.Value > .52f)
                zAxis = (controllerZ.Value - .5f) * 2f;
            else
                zAxis = 0;
        
            if (trapButton.Value == true)
                trapPress = true;
            if (leftButton.Value == true)
                leftPress = true;
            if (rightButton.Value == true)
                rightPress = true;
        
        }
        else
        {
            if (XBoxInput.GetConnected())
            {

                if (!doNotMove)
                {
                    cameraLook.x = lookSpeed * XBoxInput.GetRightX();
                    cameraLook.y = lookSpeed * XBoxInput.GetRightY() * -1;
                    zAxis = XBoxInput.GetLeftY() * -1f;
                }
                else
                {
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    zAxis = 0;
                }
        
                if (XBoxInput.GetKeyPressed(0, (int)Buttons.A))
                    trapPress = true;
                if (XBoxInput.GetKeyPressed(0, (int)Buttons.LB))
                    leftPress = true;
                if (XBoxInput.GetKeyPressed(0, (int)Buttons.RB))
                    ohmygoodness = true;
            }
            else
            {
                cameraLook.x += Input.GetAxis(mouseX);
                cameraLook.y += Input.GetAxis(mouseY);
            }
        }
        

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);
        //cameraLook.x = Mathf.Clamp(cameraLook.x, CAM_X_MIN, CAM_X_MAX);

        //Debug.Log(totalCamera.Count);
        //array will be way more useful for this stuff later on

        if (ohmygoodness)
            doTimer();

        //https://answers.unity.com/questions/615771/how-to-check-if-click-mouse-on-object.html
        //to activate trap
        if (!Input.GetMouseButton(1))
        {
            clickRay = totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter);
            //print(clickRay + " other ver " + totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter));

            if (Physics.Raycast(clickRay, out clickHit))
            {
                if (clickHit.transform.CompareTag(treasureStr))
                {
                    //atm, the overseer can only have 5 active traps at a time
                    //CmdObjectiveTrap(clickHit.collider.gameObject.name);

                    AbuttonHintUI.SetActive(true);
                    if (Input.GetMouseButtonDown(0) || (trapPress && !trapButton.Value))
                        clickHit.collider.gameObject.GetComponent<Treasure>().TreasureOnClick(theRoomManager.theImages, gameObject.GetComponent<OverSeerControl>(), theRoomManager);//, this.gameObject.GetComponent<OverSeerControl>());
                }
                else if (clickHit.transform.CompareTag(securityStr))
                {
                    //Debug.Log(camRoomName);
                    //Debug.Log(trapSelect);
                    AbuttonHintUI.SetActive(true);
                    if (Input.GetMouseButtonDown(0) || (trapPress && !trapButton.Value))
                        clickHit.collider.gameObject.GetComponent<TrapDoor>().OnSecurityClick(gameObject);
                }
                else
                    AbuttonHintUI.SetActive(false);
            }

            trapPress = false;    
        }

        for (int i = 0; i < totalCamera.Count; i++)
        {
            findFOV(i);

            if (Input.GetKeyUp(KeyCode.A) || (leftPress && leftButton.Value == false))
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    leftPress = false;
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    //totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                    totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(cameraLook.y, ogRotation[i].y + cameraLook.x, 0);

                    if (totalCamera[i] == totalCamera[0])
                    {
                        trapSelect = 17;
                        totalCamera[totalCamera.Count - 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        currentCamera = 17;
                        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
                        break;
                    }
                    else
                    {
                        trapSelect--;
                        totalCamera[i - 1].GetComponentInChildren<Camera>().enabled = true;
                        currentCamera = i - 1;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
                        break;
                    }

                    //totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                }
                theCanvasManager.UpdateCanvasCamera(totalCamera[i].GetComponentInChildren<Camera>());
            }

            if (Input.GetKeyUp(KeyCode.D) || (rightPress && rightButton.Value == false))
            {

                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    rightPress = false;
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
                    if (totalCamera[i] == totalCamera[totalCamera.Count - 1])
                    {
                        trapSelect = 0;
                        totalCamera[0].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        currentCamera = 0;
                        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
                        break;
                    }
                    else
                    {
                        trapSelect++;
                        totalCamera[i + 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        currentCamera = i + 1;
                        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
                        break;
                    }

                }
                theCanvasManager.UpdateCanvasCamera(totalCamera[i].GetComponentInChildren<Camera>());

            }

            if (controller.connected || XBoxInput.GetConnected())
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;
                    float lookSens = temp;

                    if (!radialPress)
                    {
                        if (temp < 50)
                            lookSens = temp / 1.5f;
                        if (temp >= 50)
                            lookSens = temp * 1.3f;

                        if (temp >= 30 && temp <= 70)
                        {
                            temp += zAxis * zoomSpeed * Time.deltaTime;
                            lookSpeed = 2.0f * (lookSens * 0.01f);
                        }
                        if (temp > 70)
                            temp = 70;
                        if (temp < 30)
                            temp = 30;
                    }

                    totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.W) && !Input.GetMouseButton(1))
                {
                    if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                    {
                        float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                        if (temp > 30)
                            temp -= zoomSpeed * Time.deltaTime;

                        totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                    }
                }


                if (Input.GetKey(KeyCode.S) && !Input.GetMouseButton(1))
                {
                    if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                    {
                        float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                        if (temp < 70)
                            temp += zoomSpeed * Time.deltaTime;

                        totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                    }
                }
            }

            if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
            {
                if (!Input.GetMouseButton(1))
                {
                    if (controller.connected || XBoxInput.GetConnected() /*&& !doNotMove*/)
                    {
                        float controllerRotX = totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles.x + cameraLook.y;
                        totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(Mathf.Clamp(controllerRotX, 1, 85), Mathf.Clamp(totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles.y, 0, 359) + cameraLook.x, 0);
                    }
                    else
                        totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(-cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
                }

                //Vector3 psoition = totalCamera[i].transform.position;
                //SoundManager.setListenerPos(psoition.x, psoition.y, psoition.z);
                //SoundManager.setListenerVel(0f, 0f, 0f);
                //Vector3 cast = totalCamera[i].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter).direction;
                //SoundManager.setListenerForward(cast.x, cast.y, cast.z);
            }
        }

        if (radialNeverTriggered)
        {
            radialHintTime += Time.deltaTime;

            if (radialHintTime >= 30)
                radialHintUI.SetActive(true);
        }

        if (Input.GetMouseButton(1) || radialPress)
        {
            doNotMove = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            radialCamSelect.SetActive(true);
            radialNeverTriggered = false;
            radialPress = false;

            if (radialHintUI.activeInHierarchy)
                radialHintUI.SetActive(false);
            
        }
        else if (XBoxInput.GetKeyReleased(0, (int)Buttons.RB) || Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            radialPress = false;
            doNotMove = false;
            
        }

        if (switchCameras)
        {
            radialCamSelect.SetActive(false);
            switchCameras = false;
        }



        try
        {
            if (run1 == null || run2 == null)
            {
                theCanvasManager.T3.text = "SOMETHINGS NULL BITCHHHHH";
                theCanvasManager.T4.text = "r1=" + GameObject.FindGameObjectsWithTag("Runner").Length.ToString();
                theCanvasManager.T5.text = "r2=" + GameObject.FindGameObjectsWithTag("Runner").Length.ToString();

            }
            else
            {
                if (run1.currstate != 0 || run2.currstate != 0)
                {
                    theCanvasManager.newConsoleMessage(eventConsole.repetitiveshit());
                    print("it gets to this point FUUUCk");
                    print(run1.currstate.ToString());
                    theCanvasManager.T5.text = "d " + eventConsole.repetitiveshit();
                    run1.currstate = 0;
                    run2.currstate = 0;
                }
            }
        } catch (System.IndexOutOfRangeException e)
        {
            print("The canvas manager is missing a player object (in OverseerControl) " + e.Message);
        }

        theCanvasManager.consoleMessages();

        //theCanvasManager.T5.text = "gets to this point";
        //Debug.Log(OverID);

        camRoomName = roomStr + trapSelect.ToString();

        if (!foundPlayer || !foundEnemy)
        { 
            //to find the players
            theCanvasManager.FindPlayer(team, 0, ref foundPlayer, ref foundEnemy);
            theCanvasManager.FindPlayer(team, 1, ref foundPlayer, ref foundEnemy);

        }
    }

    void doTimer()
    {
        timer += Time.deltaTime;

        if (timer < 1 && XBoxInput.GetKeyReleased(0, (int)Buttons.RB))
        {
            rightPress = true;
            ohmygoodness = false;
            timer = 0;
        }
        else if (timer >= 1)
        {
            radialPress = true;
            ohmygoodness = false;
            timer = 0;
        }
    }
    public void roleChose(OverSeerControl script)
    {
        script.camChoice = !script.camChoice;
        //script2.spawnIn = false;
        this.camChoice = script.camChoice;
     //   Debug.Log(camChoice);
    }

    public int GetNumTrap()
    {
        return theCanvasManager.numTrapActivated;
    }

    public void ObjectiveActivate(ref int currentObjectID, string location)
    {
        theCanvasManager.numTrapActivated++;
        currentObjectID = theCanvasManager.currentTrap;
        theCanvasManager.SetTrapIconActive(currentObjectID, Color.green, location);
        theCanvasManager.SetCurrentlyActive();        
    }

    public void DecoupleTrap(int currentObjectID, Color theColor, string location)
    {
        theCanvasManager.numTrapActivated--;
        theCanvasManager.SetTrapIconActive(currentObjectID, theColor, location);
        theCanvasManager.SetCurrentlyActive();
        theCanvasManager.PrintCode(gameObject.name);
    }

    [Command]
    public void CmdTrapActivate(string theName)
    {
        //camRoomName = ("Room" + trapSelect.ToString());
        //GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().trapActivated = !GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().trapActivated;
        bool ugh = !GameObject.Find(theName).GetComponent<RoomScript>().trapActivated;
        GameObject.Find(theName).GetComponent<RoomScript>().trapActivated = ugh;
        GameObject.Find(theName).GetComponent<RoomScript>().DoorTrapActive(ugh);

        //GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().doorCooldown = true;
    }

    [Command]
    public void CmdTrap(int index, bool temp)
    {
        if (theRoomManager != null)
        {
            theRoomManager.theObjectives[index].trapActive = temp;
            theRoomManager.theObjectives[index].Reshuffle(theRoomManager);
        }
    }

    [Command]
    public void CmdObjectiveTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;

    }

    [Command]
    public void CmdTrapSelect(int num)
    {
        GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway = num;
    }

    void UpdateAllUI()
    {
        for (int i = 0; i < theRoomManager.theObjectives.Length; i++)
        {
            theCanvasManager.ChangeTrapLocation(i,theRoomManager.theObjectives[i].transform.position);
        }
    }

    //the radial button calls this method to switch cameras
    public void SetCam(int theValue)
    {
        print(currentCamera);
        totalCamera[currentCamera].GetComponentInChildren<Camera>().enabled = false;
        totalCamera[theValue].GetComponentInChildren<Camera>().enabled = true;
        currentCamera = theValue;
        trapSelect = theValue;
        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());
        switchCameras = true;
        //camRoomName = 
    }

  

    [Command]
    public void CmdSetTrapActive(int trapID, bool temp)
    {
        RpcSetTrapActive(trapID, temp);
    }

    [ClientRpc]
    public void RpcSetTrapActive(int trapID, bool temp)
    {
         if (GameObject.Find("TheRoomManager"))
         { 

            theRoomManager = GameObject.Find("TheRoomManager").GetComponent<RoomManager>();
            //the line of code that was causing errors goes here
            theRoomManager.theObjectives[trapID].trapActive = temp;
         }
       
    }

    public void SetChosenTeam(int num)
    {
        team = num; //sets the team
        CmdSetTeam(team);

    }

    [Command]
    void CmdSetTeam(int num)
    {
        team = num;
        RpcSetTeam(num);
    }

    [ClientRpc]
    void RpcSetTeam(int num)
    {
        team = num;
    }
}