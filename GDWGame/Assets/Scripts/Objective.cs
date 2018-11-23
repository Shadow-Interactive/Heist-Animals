using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Objective : NetworkBehaviour {

    //Other classes/types of data contained here (components?)
    TrapBase theTrapType;
    [HideInInspector] public CodeVisual associatedCodeObject;
    OverSeerControl activeOverseer; //for the minigames
    PlayerObjectiveManager activePlayer;
    public RoomScript currentRoom;

    //public variables
    public TrapTypes objTrapType;
    [HideInInspector] public SyncListInt trapCode = new SyncListInt();
    [HideInInspector] public bool minigameActivated = false;
    [HideInInspector] public int currentObjectID = 0;

    //private/contained variables
    Vector3 objectivePosition;
    string runnerStr = "RunnerOne", runner2Str = "RunnerTwo";

    // Use this for initialization
    void Start () {
		for (int i = 0; i <4; i++)
        {
            int random = Random.Range(0, 10);
            trapCode.Add(random);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            DecoupleTrap();
        }
    }

    public void SetUpTrap(TrapBase theType)
    {
        theTrapType = theType;
        GetComponent<MeshFilter>().sharedMesh = theType.theMesh;
        gameObject.GetComponent<Renderer>().sharedMaterial = theType.theMaterial;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!minigameActivated)
        {
            if (collision.collider.CompareTag(runnerStr) || collision.collider.CompareTag(runner2Str) && objTrapType != TrapTypes.NO_TRAP)
            {
                activePlayer = collision.gameObject.GetComponentInChildren<PlayerObjectiveManager>();
                activePlayer.ActivateMinigame();
                ActivateMinigame();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (minigameActivated == true)
            minigameActivated = false;
    }

    public void Reshuffle(Texture[] theImages)
    {
        for (int i = 0; i < 4; i++)
        {
            trapCode[i] = Random.Range(0, 10);

            if (associatedCodeObject != null)
                associatedCodeObject.SetSprite(i, theImages[trapCode[i]]);
        }
    }

    public void ActivateMinigame()
    {
        minigameActivated = true;
        theTrapType.Execute(activePlayer);
    }

    public void ActivateObject(Texture[] theImages,OverSeerControl activeO)
    {
         activeOverseer = activeO;

         if (activeOverseer.GetNumTrap() < 4)
         {
            GameObjectVisible(true);
            activeOverseer.ObjectiveActivate(ref currentObjectID, GetRoomID());
            Reshuffle(theImages);
        }
    }
    
    public void GameObjectVisible(bool temp) //this controls the visibility
    {
        gameObject.SetActive(temp);
        if  (associatedCodeObject != null)
        associatedCodeObject.SetActive(temp);
    }

    public void DecoupleTrap() //This handles the UI side of the minigame
    {
        if (activeOverseer != null && activeOverseer.GetNumTrap() > 0)
        {
            activeOverseer.DecoupleTrap(currentObjectID, Color.red, GetRoomID());
        }

        GameObjectVisible(false);
    }

    public int GetRoomID()
    {
        return currentRoom.roomTag;
    }
}
