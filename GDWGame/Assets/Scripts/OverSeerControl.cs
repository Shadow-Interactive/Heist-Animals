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

    private const float CAM_Y_MIN = -80.0f;
    private const float CAM_Y_MAX = 20.0f;
    private const float CAM_X_MIN = -40.0f;
    private const float CAM_X_MAX = 40.0f;

    public float test;

    private float zoomSpeed = 80.0f;
    private float zAxis;

    //can optimize later
    public GameObject cam1, cam2, cam3, cam4, cam5, cam6, cam7, cam8, cam9, cam10, cam11, cam12, cam13, cam14, cam15;

    private List<GameObject> totalCamera = new List<GameObject>();

    private Vector2 cameraLook;
    private List<Vector3> ogRotation = new List<Vector3>();

    public bool camChoice = true;
    bool initRM = false;
    int trapSelect = 0;
    
    string mouseX = "Mouse X", mouseY = "Mouse Y";
    string treasureStr = "Treasure", securityStr = "SecurityBox";
    string networkTrapStr = "TheNetworkTrap", roomStr = "Room",
        O1 = "Objective", O2 = "Objective2", O3 = "Objective3", O4 = "Objective4", O5 = "Objective5", O6 = "Objective5", O7 = "Objective7";
    public string camRoomName;

    RaycastHit clickHit;
    Ray clickRay;

    RoomManager theRoomManager;
    
    Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0);

    int currentCamera = 0;

	[SyncVar] public int OverID;
	PlayerLogic run1; //friendly
	PlayerLogic run2; //enemy
	ObserverPattern.EventConsole eventConsole;

	public OverseerCanvasManager theCanvasManager;

    [HideInInspector] public Objective R1currentObjective;
    [HideInInspector] public Objective R2currentObjective;

    public Material run2mat;

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
        totalCamera.Add(cam13);
        totalCamera.Add(cam14);
        totalCamera.Add(cam15);

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

        if (OverID == 1)
        {
            if (camChoice)
                totalCamera[3].GetComponentInChildren<Camera>().enabled = true;
            else
                totalCamera[3].GetComponentInChildren<Camera>().enabled = false;

            for (int i = 0; i < 3; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }

            for (int i = 4; i < totalCamera.Count; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }
            camRoomName = "Room4";
            trapSelect = 4;
        }
        else if (OverID == 2)
        {
            if (camChoice)
                totalCamera[11].GetComponentInChildren<Camera>().enabled = true;
            else
                totalCamera[11].GetComponentInChildren<Camera>().enabled = false;

            for (int i = 0; i < 11; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }

            for (int i = 12; i < totalCamera.Count; i++)
            {
                totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
            }
            camRoomName = "Room12";
            trapSelect = 12;
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
        if (OverID == 1)
            currentCamera = 3;
        else if (OverID == 2)
            currentCamera = 11;
        gameObject.transform.position = new Vector3(9.5f, -1.5f, 1.1f);

        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
       
        //cursor.GetComponent<RectTransform>().anchoredPosition = viewportCenter;
        theCanvasManager.LoadProperties(theRoomManager);

        initRM = true;
        theCanvasManager.SwitchCameras(currentCamera, totalCamera[currentCamera].GetComponentInChildren<Camera>());


		//BECAUSE IT MIGHTVE BEEN MAKING MULTIPLE STUFF
		//the dumbstuff
		run1 = GameObject.FindGameObjectsWithTag("RunnerOne")[0].GetComponent<PlayerLogic>();
		run2 = GameObject.FindGameObjectsWithTag("RunnerOne")[1].GetComponent<PlayerLogic>();


		if (OverID == 1)
		{
			eventConsole = new ObserverPattern.EventConsole(run1, run2);
			Debug.Log("event console 1 assigned");
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

        GameObject.FindGameObjectsWithTag("RunnerOne")[1].GetComponentInChildren<SkinnedMeshRenderer>().material = run2mat;

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
      //  else
      //  {
            cameraLook.x += Input.GetAxis(mouseX);
            cameraLook.y += Input.GetAxis(mouseY);
    //    }

        //clamp the camera's y rotation to prevent weird camera angles
        cameraLook.y = Mathf.Clamp(cameraLook.y, CAM_Y_MIN, CAM_Y_MAX);
        //cameraLook.x = Mathf.Clamp(cameraLook.x, CAM_X_MIN, CAM_X_MAX);

        //Debug.Log(totalCamera.Count);
        //array will be way more useful for this stuff later on

        //https://answers.unity.com/questions/615771/how-to-check-if-click-mouse-on-object.html
        //to activate trap
        if (Input.GetMouseButtonDown(0) || ( trapPress && !trapButton.Value))
        {
            trapPress = false;
            clickRay = totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter);
            //print(clickRay + " other ver " + totalCamera[currentCamera].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter));

            if (Physics.Raycast(clickRay, out clickHit))
            {
                if (clickHit.transform.CompareTag(treasureStr))
                {
                    //atm, the overseer can only have 5 active traps at a time
                    //CmdObjectiveTrap(clickHit.collider.gameObject.name);
                    clickHit.collider.gameObject.GetComponent<Treasure>().TreasureOnClick(theRoomManager.theImages, gameObject.GetComponent<OverSeerControl>(), theRoomManager);//, this.gameObject.GetComponent<OverSeerControl>());
                }
                else if (clickHit.transform.CompareTag(securityStr))
                {
                    //Debug.Log(camRoomName);
                    //Debug.Log(trapSelect);
                    clickHit.collider.gameObject.GetComponent<TrapDoor>().OnSecurityClick(gameObject);
                }
            }
           
        }

        for (int i = 0; i < totalCamera.Count; i++)
        {
            findFOV(i);

            if (Input.GetKeyUp(KeyCode.LeftArrow) || (leftPress && leftButton.Value == false))
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
                        trapSelect = 15;
                        totalCamera[totalCamera.Count - 1].GetComponentInChildren<Camera>().enabled = true;
                        totalCamera[i].GetComponentInChildren<Camera>().enabled = false;
                        currentCamera = 14;
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

            if (Input.GetKeyUp(KeyCode.RightArrow) || (rightPress && rightButton.Value == false))
            {
                
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    rightPress = false;
                    cameraLook.x = 0;
                    cameraLook.y = 0;
                    totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
                    if (totalCamera[i] == totalCamera[totalCamera.Count - 1])
                    {
                        trapSelect = 1;
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

            if (controller.connected)
            {
                if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                {
                    float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                    if (temp >= 30 && temp <= 70)
                        temp -= zAxis * zoomSpeed * Time.deltaTime;
                    if (temp > 70)
                        temp = 70;
                    if (temp < 30)
                        temp = 30;

                    totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (totalCamera[i].GetComponentInChildren<Camera>().enabled)
                    {
                        float temp = totalCamera[i].GetComponentInChildren<Camera>().fieldOfView;

                        if (temp > 30)
                            temp -= zoomSpeed * Time.deltaTime;

                        totalCamera[i].GetComponentInChildren<Camera>().fieldOfView = temp;
                    }
                }


                if (Input.GetKey(KeyCode.DownArrow))
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
                if (controller.connected)
                {
                    totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(Mathf.Clamp(totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles.x, 1, 85) + cameraLook.y, Mathf.Clamp(totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles.y, 0, 359) + cameraLook.x, 0);
                }
                else
                    totalCamera[i].GetComponentInChildren<Camera>().transform.eulerAngles = new Vector3(-cameraLook.y, ogRotation[i].y + cameraLook.x, 0);
                Vector3 psoition = totalCamera[i].transform.position;
                SoundManager.setListenerPos(psoition.x, psoition.y, psoition.z);
                SoundManager.setListenerVel(0f, 0f, 0f);
                Vector3 cast = totalCamera[i].GetComponentInChildren<Camera>().ViewportPointToRay(viewportCenter).direction;
                SoundManager.setListenerForward(cast.x, cast.y, cast.z);
            }
        }


		//EVENT CONSOLE BULLSHIT
		//	to send stuff on the canvas queue
		//if (eventConsole.repetitiveshit() != null)
		//{
		//	theCanvasManager.newConsoleMessage(eventConsole.repetitiveshit());
		//	//print("it gets to this point FUUUCk");
		//}
		//print(eventConsole.repetitiveshit());

		theCanvasManager.T1.text ="r1="+GameObject.FindGameObjectsWithTag("RunnerOne").Length.ToString();
		theCanvasManager.T2.text ="r2=" + GameObject.FindGameObjectsWithTag("RunnerTwo").Length.ToString();

		if (run1 == null || run2 == null)
		{
			theCanvasManager.T3.text = "SOMETHINGS NULL BITCHHHHH";
			theCanvasManager.T4.text = "r1=" + GameObject.FindGameObjectsWithTag("RunnerOne").Length.ToString();
			theCanvasManager.T5.text = "r2=" + GameObject.FindGameObjectsWithTag("RunnerTwo").Length.ToString();

		}
		else
		{
			theCanvasManager.T4.text = "gets to this point";
			if (OverID == 1)
			{
				theCanvasManager.T5.text = run1.currstate.ToString();
			}
			else if (OverID == 2)
			{
				theCanvasManager.T5.text = run2.currstate.ToString();
			}
		}
		//theCanvasManager.T5.text = "gets to this point";



		//Debug.Log(OverID);

		camRoomName = roomStr + trapSelect.ToString();
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
        //camRoomName = ("Room" + trapSelect.ToString());
        //GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().trapActivated = !GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().trapActivated;
        theRoomManager.theObjectives[index].trapActive = temp;
        //GameObject.FindGameObjectWithTag("C" + trapSelect.ToString()).GetComponent<RoomScript>().doorCooldown = true;
    }

    [Command]
    public void CmdObjectiveTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
        //currentObjective.trapActive = !currentObjective.trapActive;
        //currentObjective.DeActivate();
    }

    [Command]
    public void CmdTrapSelect(int num)
    {
        GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway = num;
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        //GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
        //currentObjective.trapActive = !currentObjective.trapActive;
        //currentObjective.DeActivate();

    }
}