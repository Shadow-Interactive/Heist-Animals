using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class OverseerCanvasManager : NetworkBehaviour
{
    //for the trap
    public GameObject screenBlocker;
    bool EMP = false;
    float empTimer = 0;

    //all the juicy ui stuff
    public Canvas trapCanvas, regularCanvas;
    private List<GameObject> codeVisuals = new List<GameObject>();
    private List<GameObject> trapImages = new List<GameObject>();

    GameObject[] theImages = new GameObject[4];
    GameObject codePrefab, playerDUI, enemyDUI, pDUI, eDUI;
    GameObject shockImage, empImage;
    GameObject imageTemp;

    private float imageHeight;
    
    [HideInInspector] public int numTrapActivated = 0, currentTrap = 0;
    public RawImage minimapPoint;
    public RawImage[] trapIcons = new RawImage[4];

    Vector3[] pointPositions = new Vector3[18];
   
    public Text currentRoom;

	public Text T1;
	public Text T2;
	public Text T3;
	public Text T4;
	public Text T5;
	Queue<string> messageQ = new Queue<string>();

	string[] theRoomTexts = new string[18];
    string roomStr = "Room: ", defaultText = "NA", strO1 = "Overseer1", strO2 = "Overseer2", strRunner = "Runner";

    public Canvas cursorCanvas;
    public int overseerID;

    // Use this for initialization
    void Start () {

        if (overseerID == 0)
            gameObject.name = strO1;
        else if (overseerID == 1)
            gameObject.name = strO2;
        else
            gameObject.name = "killmebish" + overseerID; //ohno
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    //for ui icons to show up
    public void FindPlayer(int currentTeam, int runnerIndex, ref bool foundPlayer, ref bool foundEnemy)
    {
        try
        {
            if (GameObject.FindGameObjectsWithTag(strRunner)[runnerIndex] != null)
            {
                //makes code a little cleaner
                PlayerLogic runner1 = GameObject.FindGameObjectsWithTag(strRunner)[runnerIndex].GetComponent<PlayerLogic>();
                Vector3 position = runner1.gameObject.transform.position;

                if (runner1.team == currentTeam && foundPlayer == false)
                {
                    //this is for the diagetic ui
                    pDUI = GameObject.Instantiate(playerDUI);
                    pDUI.transform.SetParent(trapCanvas.transform);
                    pDUI.transform.position = new Vector3(position.x, 3, position.z);
                    runner1.pDUI = pDUI;

                    //for the map ui
                    RunnerLocUI theRunnerUI = GetComponentInChildren<RunnerLocUI>();
                    runner1.runnerLocationUI = theRunnerUI;

                    //for the rim lighting
                    runner1.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetColor("_RimLight", Color.green);
                    runner1.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetFloat("_RimStrength", 2.0f);

                    //establish that we've found the player
                    foundPlayer = true;
                }
                else if (runner1.team != currentTeam && foundEnemy == false)
                {
                    //looks for enemy
                    eDUI = GameObject.Instantiate(enemyDUI);
                    eDUI.transform.SetParent(trapCanvas.transform);
                    eDUI.transform.position = new Vector3(position.x, 3, position.z);
                    runner1.eDUI = eDUI; //to update the enemy pos in diagetic canvas

                    //for the rim lighting
                    runner1.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetColor("_RimLight", Color.red);
                    runner1.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetFloat("_RimStrength", 2.0f);

                    //signify that we found the enemy
                    foundEnemy = true;
                }
            }
        }
        catch
        {
            //catch statement goes here
        }

    }

    public void ChangeBillboard(Camera newCamera)
    {
        //setting new cam positions, this is for the billboard
        for(int i = 0; i < codeVisuals.Count; i++)
        {
            codeVisuals[i].GetComponent<CameraBillboard>().theCamera = newCamera;
        }
        if (pDUI != null) pDUI.GetComponent<DUIScript>().theCamera = newCamera;
        if (eDUI != null) eDUI.GetComponent<DUIScript>().theCamera = newCamera;

    }

    public void LoadProperties(RoomManager theRoomManager)
    {

        //print(overseerID + " the id");
        codePrefab = Resources.Load("ONumberOutput") as GameObject;
        playerDUI = Resources.Load("playerDUI") as GameObject;
        enemyDUI = Resources.Load("enemyDUI") as GameObject;
        shockImage = Resources.Load("ShockUI") as GameObject;
        empImage = Resources.Load("EMPUI") as GameObject;

        //we need images or smth to dictate no traps/safeguards
        theImages[0] = shockImage;
        theImages[1] = shockImage;
        theImages[2] = empImage;
        theImages[3] = empImage;

        //adds the traps and associated images
        for (int i = 0; i < theRoomManager.ObjectiveLength(); i++)
        {
            AddTrap(theRoomManager.GetObjPos(i), theRoomManager.GetObjCode(i), i, theRoomManager);
        }

        for (int i = 0; i < theRoomManager.SecurityLength(); i++)
        {
            AddImage(theRoomManager.GetSecurityPos(i), theRoomManager.GetSecurityRotation(i), theRoomManager.getRoomTrap(i));
        }

        //setting the red dot positions
        pointPositions[0] = new Vector3(3.03f, 3.76f, 0); //c
        pointPositions[1] = new Vector3(12.3f, 1.79f, 0); //c
        pointPositions[2] = new Vector3(-3.1f, -14.98f, 0); //c
        pointPositions[3] = new Vector3(-19.5f, -17.6f, 0); //c
        pointPositions[4] = new Vector3(-31.3f, 10.3f, 0); //c
        pointPositions[5] = new Vector3(-22.9f, 32f, 0);//c
        pointPositions[6] = new Vector3(-8.7f, -27.6f, 0); //c
        pointPositions[7] = new Vector3(2.6f, 17.6f, 0); //c
        pointPositions[8] = new Vector3(13.1f, 20.5f, 0); //c
        pointPositions[9] = new Vector3(32.9f, 17.5f, 0); //c
        pointPositions[10] = new Vector3(19, 0.76f, 0); //c
        pointPositions[11] = new Vector3(14.7f, -27.2f, 0);
        pointPositions[12] = new Vector3(1.6f, -38.03f, 0); // c
        pointPositions[13] = new Vector3(-7.9f, -29.6f, 0); //c
        pointPositions[14] = new Vector3(-23.4f, -42.8f, 0); //c
        pointPositions[15] = new Vector3(-35.9f, -22.9f, 0); //c
        pointPositions[16] = new Vector3(-40.3f, -10.7f, 0);//c
        pointPositions[17] = new Vector3(-29.55f, 3.6f, 0);  //c
        
        //the text for all the rooms
        theRoomTexts[0] = "Current Camera: 1";
        theRoomTexts[1] = "Current Camera: 2";
        theRoomTexts[2] = "Current Camera: 3";
        theRoomTexts[3] = "Current Camera: 4";
        theRoomTexts[4] = "Current Camera: 5";
        theRoomTexts[5] = "Current Camera: 6";
        theRoomTexts[6] = "Current Camera: 7";
        theRoomTexts[7] = "Current Camera: 8";
        theRoomTexts[8] = "Current Camera: 9";
        theRoomTexts[9] = "Current Camera: 10";
        theRoomTexts[10] ="Current Camera: 11";
        theRoomTexts[11] ="Current Camera: 12";
        theRoomTexts[12] ="Current Camera: 13";
        theRoomTexts[13] ="Current Camera: 14";
        theRoomTexts[14] ="Current Camera: 15";
        theRoomTexts[15] ="Current Camera: 16";
        theRoomTexts[16] ="Current Camera: 17";
        theRoomTexts[17] = "Current Camera: 18";
    }
    
    public void ChangeTrapLocation(int index, Vector3 position) //for when the objective is dropping
    {
        codeVisuals[index].transform.position = new Vector3(position.x, 2.5f, position.z);
    }

    void AddTrap(Vector3 trapPos, SyncListInt theCode, int codeIndex, RoomManager theRoomManager )
    {
        GameObject trapObj = GameObject.Instantiate(codePrefab);
        //trapObj.transform.parent = trapCanvas.transform;
        trapObj.transform.SetParent(trapCanvas.transform);
        trapObj.transform.position = new Vector3(trapPos.x, 4, trapPos.z);
        trapObj.GetComponent<CodeVisual>().SetIndex(codeIndex);

        if (gameObject.name == strO1)
            theRoomManager.theObjectives[codeIndex].associatedCodeObject1 = trapObj.GetComponent<CodeVisual>();

        else if (gameObject.name == strO2)
            theRoomManager.theObjectives[codeIndex].associatedCodeObject2 = trapObj.GetComponent<CodeVisual>();
        
        codeVisuals.Add(trapObj);
    }

    void AddImage(Transform boxPos, Vector3 boxRotation, RoomTraps theType)
    {
        imageTemp = GameObject.Instantiate(theImages[(int)theType]);
        imageTemp.transform.position = new Vector3(boxPos.position.x, boxPos.position.y + 5f, boxPos.position.z);
        imageTemp.transform.eulerAngles = new Vector3(boxRotation.x, boxRotation.y + 90, boxRotation.z);
        imageTemp.transform.parent = trapCanvas.transform;
        trapImages.Add(imageTemp);
    }

    void SetPointPosition(int positionIndex)
    {
        minimapPoint.GetComponent<RectTransform>().anchoredPosition = pointPositions[positionIndex];
    }

    public void SetTrapIconActive(int trapIndex, Color activeColor, string location)
    {
        trapIcons[trapIndex].color = activeColor;

        if (activeColor != Color.red)
            trapIcons[trapIndex].GetComponentInChildren<Text>().text = location;
        else
            trapIcons[trapIndex].GetComponentInChildren<Text>().text = defaultText;

    }

    public void SetCurrentlyActive()
    {
        for (int i = 0; i < 4; i++)
        {
            if (trapIcons[i].color != Color.green)
            {
                currentTrap = i;
                return;
            }
        }
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

    public void SwitchCameras(int currentCamera, Camera theCamera)
    {
        currentRoom.text = theRoomTexts[currentCamera];
        SetPointPosition(currentCamera);
        cursorCanvas.worldCamera = theCamera;
        ChangeBillboard(theCamera);
    }

    public void UpdateCanvasCamera(Camera newCamera)
    {
        trapCanvas.worldCamera = newCamera;
    }

    public void SetCanvasActive(bool temp)
    {
        regularCanvas.gameObject.SetActive(temp);
        trapCanvas.gameObject.SetActive(temp);
    }

    //these are for testing
   public void PrintCode(SyncListInt trapCode)
    {
        currentRoom.text = (trapCode[0] + " " + trapCode[1] + " " + trapCode[2] + " " + trapCode[3]).ToString();
    }

    public void PrintCode(int num)
    {
        currentRoom.text = num.ToString();
    }

    public void PrintCode(bool temp)
    {
        currentRoom.text = temp.ToString();
    }

    public void PrintCode(string help)
    {
        currentRoom.text = help;

    }

	//call in the update function
	public void consoleMessages()
	{
		if (messageQ.Count == 0)
		{
			messageQ.Enqueue("1");
			messageQ.Enqueue("2");
			messageQ.Enqueue("3");
			messageQ.Enqueue("4");
			//	messageQ.Enqueue("");

			//Debug.Log("it gets to this point");
		}
		if (messageQ.Count > 4)
		{
			//clean this shit up
			removeOldConsoleMessage();
		}

		//top to bottom t4 the newes
		T1.text = messageQ.ElementAt(3);
		T2.text = messageQ.ElementAt(2);
		T3.text = messageQ.ElementAt(1);
		T4.text = messageQ.ElementAt(0);
		//T5.text = messageQ.Count.ToString();


	}

	//puts a new message on the q to display
	public void newConsoleMessage(string k)
	{
		messageQ.Enqueue(k);
	}

	//removes old messages on the q
	public void removeOldConsoleMessage()
	{
		messageQ.Dequeue();
	}

    
}
