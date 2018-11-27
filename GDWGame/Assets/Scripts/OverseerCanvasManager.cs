using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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

    public GameObject emptyTrapIcons;
    GameObject[] theImages = new GameObject[4];
    GameObject codePrefab;
    GameObject shockImage, empImage;
    GameObject imageTemp;

    private float imageHeight;
    
    [HideInInspector] public int numTrapActivated = 0, currentTrap = 0;
    public RawImage minimapPoint;
    RawImage[] trapIcons;

    Vector3[] pointPositions = new Vector3[15];
    public Text currentRoom;
    
    string[] theRoomTexts = new string[15];
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

      //  currentRoom.text = gameObject.name;

    }

    public void LoadProperties(RoomManager theRoomManager)
    {

        print(overseerID + " the id");
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

        //more ui stuff here
        trapIcons = emptyTrapIcons.GetComponentsInChildren<RawImage>();

        pointPositions[0] = new Vector3(-7.9f, -5.4f, 0);
        pointPositions[1] = new Vector3(13.9f, -5.4f, 0);
        pointPositions[2] = new Vector3(32.5f, -5.5f, 0);
        pointPositions[3] = new Vector3(31.2f, -30.4f, 0);
        pointPositions[4] = new Vector3(13.4f, 4.9f, 0);
        pointPositions[5] = new Vector3(-6, 20.9f, 0);
        pointPositions[6] = new Vector3(32, 21.3f, 0);
        pointPositions[7] = new Vector3(21.6f, 33.2f, 0);
        pointPositions[8] = new Vector3(31.1f, 10.9f, 0);
        pointPositions[9] = new Vector3(-25.7f, 0, 0);
        pointPositions[10] = new Vector3(-40.6f, 25.9f, 0);
        pointPositions[11] = new Vector3(-43.5f, -2.1f, 0);
        pointPositions[12] = new Vector3(-10.6f, -15f, 0);
        pointPositions[13] = new Vector3(-10.6f, -30.4f, 0);
        pointPositions[14] = new Vector3(-39.6f, -30.4f, 0);

        theRoomTexts[0] = "Room 8";
        theRoomTexts[1] = "Room 9";
        theRoomTexts[2] = "Hallway 1";
        theRoomTexts[3] = "Room 6";
        theRoomTexts[4] = "Hallway 2";
        theRoomTexts[5] = "Room 1";
        theRoomTexts[6] = "Hallway 3";
        theRoomTexts[7] = "Room 8";
        theRoomTexts[8] = "Room 9";
        theRoomTexts[9] = "Hallway 4";
        theRoomTexts[10] = "Room 2";
        theRoomTexts[11] = "Room 3";
        theRoomTexts[12] = "Hallway 5";
        theRoomTexts[13] = "Room 5";
        theRoomTexts[14] = "Room 4";


       // currentRoom.text = gameObject.name;
    }

    void AddTrap(Vector3 trapPos, SyncListInt theCode, int codeIndex, RoomManager theRoomManager )
    {
        GameObject trapObj = GameObject.Instantiate(codePrefab);
        trapObj.transform.parent = trapCanvas.transform;
        trapObj.transform.position = new Vector3(trapPos.x, -1, trapPos.z);
        trapObj.GetComponent<CodeVisual>().SetIndex(codeIndex);

        if (gameObject.name == strO1)
        theRoomManager.theObjectives[codeIndex].associatedCodeObject1 = trapObj.GetComponent<CodeVisual>();

        else if (gameObject.name == strO2)
            theRoomManager.theObjectives[codeIndex].associatedCodeObject2 = trapObj.GetComponent<CodeVisual>();

        for (int i = 0; i < 4; i++)
        {
            // print(theRoomManager.theImages[theCode[i]]);
            trapObj.GetComponent<CodeVisual>().SetSprite(i, theRoomManager.theImages[theCode[i]]);// theRoomManager.theImages[theCode[i]]);
        }

        codeVisuals.Add(trapObj);
    }

    void AddImage(Transform boxPos, Vector3 boxRotation, RoomTraps theType)
    {
        imageTemp = GameObject.Instantiate(theImages[(int)theType]);
        imageTemp.transform.position = new Vector3(boxPos.position.x, boxPos.position.y + 1.7f, boxPos.position.z);
        imageTemp.transform.eulerAngles = new Vector3(boxRotation.x, boxRotation.y + 90, boxRotation.z);
        imageTemp.transform.parent = trapCanvas.transform;
        trapImages.Add(imageTemp);
    }

    void SetPointPosition(int positionIndex)
    {
        minimapPoint.GetComponent<RectTransform>().anchoredPosition = pointPositions[positionIndex];
    }

    public void SetTrapIconActive(int trapIndex, Color activeColor, int roomNum)
    {
        trapIcons[trapIndex].color = activeColor;

        if (activeColor != Color.red)
            trapIcons[trapIndex].GetComponentInChildren<Text>().text = roomStr + roomNum.ToString();
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

   public void PrintCode(SyncListInt trapCode)
    {
        currentRoom.text = (trapCode[0] + " " + trapCode[1] + " " + trapCode[2] + " " + trapCode[3]).ToString();
    }
}
