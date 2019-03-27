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
    GameObject codePrefab;
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
    string roomStr = "Room: ", defaultText = "NA", strO1 = "Overseer1", strO2 = "Overseer2";

    public Canvas cursorCanvas;
    public int overseerID;
    

    // Use this for initialization
    void Start () {

        if (overseerID == 0)
            gameObject.name = strO1;
        else if (overseerID == 1)
            gameObject.name = strO2;
        else
            gameObject.name = "killmebish" + overseerID;
    }
	
	// Update is called once per frame
	void Update () {
       
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
		//consoleMessages();
		//Debug.Log(messageQ.Count);

		//  currentRoom.text = gameObject.name;
	}

    public void ChangeBillboard(Camera newCamera)
    {
        for(int i = 0; i < codeVisuals.Count; i++)
        {
            codeVisuals[i].GetComponent<CameraBillboard>().theCamera = newCamera;
        }
    }

	public void LoadProperties(RoomManager theRoomManager)
    {

        //print(overseerID + " the id");
        codePrefab = Resources.Load("ONumberOutput") as GameObject;
        shockImage = Resources.Load("ShockUI") as GameObject;
        empImage = Resources.Load("EMPUI") as GameObject;

        //we need images or smth to dictate no traps/safeguards
        theImages[0] = shockImage;
        theImages[1] = shockImage;
        theImages[2] = empImage;
        theImages[3] = empImage;

        for (int i = 0; i < theRoomManager.ObjectiveLength(); i++)
        {
            AddTrap(theRoomManager.GetObjPos(i), theRoomManager.GetObjCode(i), i, theRoomManager);

        }

        for (int i = 0; i < theRoomManager.SecurityLength(); i++)
        {
            AddImage(theRoomManager.GetSecurityPos(i), theRoomManager.GetSecurityRotation(i), theRoomManager.getRoomTrap(i));
        }


        //pointPositions[0] = new Vector3(-3.5f, -3.4f, 0);
        //pointPositions[1] = new Vector3(-2.3f, -9.3f, 0);
        //pointPositions[2] = new Vector3(11, -3.1f, 0);
        //pointPositions[3] = new Vector3(-2.7f, 11.3f, 0);
        //pointPositions[4] = new Vector3(-15.3f, 3.2f, 0);
        //pointPositions[5] = new Vector3(-20.1f, -18.3f, 0);
        //pointPositions[6] = new Vector3(-21.7f, -29.8f, 0);
        //pointPositions[7] = new Vector3(-7.9f, -29.8f, 0);
        //pointPositions[8] = new Vector3(2.6f, -24.2f, 0);
        //pointPositions[9] = new Vector3(-13.5f, -24.2f, 0);
        //pointPositions[10] = new Vector3(-23.8f, -24.2f, 0);
        //pointPositions[11] = new Vector3(18.7f, -6.6f, 0);
        //pointPositions[12] = new Vector3(15.8f, 21.3f, 0);
        //pointPositions[13] = new Vector3(3.1f, 21.3f, 0);
        //pointPositions[14] = new Vector3(-7.9f, 21.3f, 0);
        //pointPositions[15] = new Vector3(-18, 31.2f, 0);
        //pointPositions[16] = new Vector3(-27.1f, 16.9f, 0);//current
        //pointPositions[17] = new Vector3(-27.1f, 2.9f, 0);

        //pointPositions[0] = new Vector3(-3.5f, -3.4f, 0); //c
        //pointPositions[1] = new Vector3(11.1f, -9.1f, 0); //c
        //pointPositions[2] = new Vector3(-3.1f, 11.2f, 0); //c
        //pointPositions[3] = new Vector3(-15, -12.2f, 0); //c
        //pointPositions[4] = new Vector3(-19.4f, -17.2f, 0); //c
        //pointPositions[5] = new Vector3(-22.9f, -29.6f, 0);//c
        //pointPositions[6] = new Vector3(-7.5f, -29.6f, 0); //c
        //pointPositions[7] = new Vector3(2.6f, -24.2f, 0); //c
        //pointPositions[8] = new Vector3(13.1f, -23.2f, 0); //c
        //pointPositions[9] = new Vector3(23.4f, -26.3f, 0); //c
        //pointPositions[10] = new Vector3(19, -7.1f, 0); //c
        //pointPositions[11] = new Vector3(14.7f, 26, 0);
        //pointPositions[12] = new Vector3(3.3f, 22.9f, 0); // c
        //pointPositions[13] = new Vector3(-7.9f, 21.3f, 0); //c
        //pointPositions[14] = new Vector3(-17.9f, 32.1f, 0); //c
        //pointPositions[15] = new Vector3(-26.5f, 17.9f, 0); //c
        //pointPositions[16] = new Vector3(-26.5f, 3.6f, 0);//c
        //pointPositions[17] = new Vector3(-25f, -7f, 0);  //c

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

        //pointPositions[11] = new Vector3(23.4f, -23.6f, 0);//current
        //pointPositions[10] = new Vector3(14, -23.6f, 0);
        //12 and fifteen flipped
        //7 and 8 flipperd
        //14 and 13 flipped

        //room 5 is 10
        //room 6 is 11 (the current room 12)

        //11 and 16?
        //17 and 10?
        //6 and 9?

        //theRoomTexts[0] = "Foyer";
        //theRoomTexts[1] = "Hallway 1";
        //theRoomTexts[2] = "Hallway 2";
        //theRoomTexts[3] = "Hallway 3";
        //theRoomTexts[4] = "Hallway 4";
        //theRoomTexts[5] = "Room 1";
        //theRoomTexts[6] = "Room 2";
        //theRoomTexts[7] = "Room 3";
        //theRoomTexts[8] = "Room 4";
        //theRoomTexts[9] = "Room 5";
        //theRoomTexts[10] = "Room 6";
        //theRoomTexts[11] = "Room 7";
        //theRoomTexts[12] = "Room 8";
        //theRoomTexts[13] = "Room 9";
        //theRoomTexts[14] = "Room 10";
        //theRoomTexts[15] = "Room 11";
        //theRoomTexts[16] = "Room 12";
        //theRoomTexts[17] = "Room 13";

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

        //i dont think we need this here, it was giving me errors
      //for (int i = 0; i < 4; i++)
      //{
      //    print(theRoomManager.theImages[theCode[i]]);
      //    trapObj.GetComponent<CodeVisual>().SetSprite(i, theRoomManager.theImages[theCode[0]]);// theRoomManager.theImages[theCode[i]]);
      //}

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
			//T1.text = messageQ.ElementAt(0);
			//T2.text = messageQ.ElementAt(1);
			//T3.text = messageQ.ElementAt(2);
			//T4.text = messageQ.ElementAt(3);
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
