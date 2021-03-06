﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Objective : NetworkBehaviour {

    //Other classes/types of data contained here (components?)
    TrapBase theTrapType;
    [HideInInspector] public CodeVisual associatedCodeObject1, associatedCodeObject2;
    OverSeerControl activeOverseer; //for the minigames
    PlayerObjectiveManager activePlayer;
    public RoomScript currentRoom;

    PlayerLogic runner1, runner2; //players for syncing objectives

    //public variables
    [SyncVar] public TrapTypes objTrapType;
    [HideInInspector] public SyncListInt trapCode = new SyncListInt();
    [SyncVar] [HideInInspector] public bool trapActive = true;
    public int trapNum = 0;
    [HideInInspector] [SyncVar] public bool minigameActivated = false;
    [HideInInspector] public int currentObjectID = 0;

    //private/contained variables
    Vector3 objectivePosition;
    string runnerStr = "RunnerOne", runner2Str = "RunnerTwo";

    [SyncVar] public int trapID = 0;

    // Use this for initialization
    void Start () {
		for (int i = 0; i <4; i++)
        {
            int random = Random.Range(0, 10);
            trapCode.Add(random);
        }

        print("New trap is " + trapCode[0] + trapCode[1] + trapCode[2] + trapCode[3]);
    }

    private void Update()
    {
      //if (Input.GetKeyDown("i"))
      //{
      //    RpcDecoupleTrap();
      //}

        GameObjectVisibleNetwork();

        //Debug.Log(trapActive);
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

				activePlayer.thePlayer.currstate = 4;
			}
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (minigameActivated == true)
            minigameActivated = false;
    }

    public void Reshuffle(RoomManager theRoomManager)
    {
        theRoomManager.activateReshuffle = true;
        theRoomManager.reshuffleID = trapID;
       // print("New trap is " + trapCode[0] + trapCode[1] + trapCode[2] + trapCode[3]);
    }

    public void ActualReshuffle()
    {
         for (int i = 0; i < 4; i++)
         {
             trapCode[i] = Random.Range(0, 10);
            
         }
        
        print("actual trap is " + trapCode[0] + trapCode[1] + trapCode[2] + trapCode[3]);
    }

    public void UpdateSprites(Texture[] theImages)
    {
        for (int i = 0; i < 4; i++)
        {
            if (associatedCodeObject1 != null)
            {
               // associatedCodeObject1.GetComponentInParent<OverseerCanvasManager>().PrintCode(trapCode);
                associatedCodeObject1.SetSprite(i, theImages[trapCode[i]]);
            }

            if (associatedCodeObject2 != null)
            {
                //associatedCodeObject1.GetComponentInParent<OverseerCanvasManager>().PrintCode(trapCode);
                associatedCodeObject2.SetSprite(i, theImages[trapCode[i]]);
            }
        }
    }

    public void ActivateMinigame()
    {
        minigameActivated = true;
        theTrapType.Execute(activePlayer);
    }

    public void ActivateObject(OverSeerControl activeO, RoomManager theRoomManager)
    {
        if (activeO == null)
        {
            print("dsadsada");
        }
         activeOverseer = activeO;

         if (activeOverseer.GetNumTrap() < 4)
         {
            //hacky fun
            CmdSetTrapActive(true);

            if (runner1 != null) runner1.CmdActivateTrap(gameObject.name);
            if (runner2 != null) runner2.CmdActivateTrap(gameObject.name);
            activeOverseer.CmdTrap(trapID, true);
            GameObjectVisible(true);

            activeOverseer.CmdTrapSelect(trapNum);
            activeOverseer.ObjectiveActivate(ref currentObjectID, GetRoomID());
            Reshuffle(theRoomManager);
        }
    }

    [Command]
    public void CmdSetTrapActive(bool temp)
    {
        trapActive = temp;
        RpcSetTrapActive(temp);
    }

    [ClientRpc]
    public void RpcSetTrapActive(bool temp)
    {
        trapActive = temp;
    }

    [Command]
    public void CmdDecouple()
    {
        RpcDecouple();
    }

    [ClientRpc]
    public void RpcDecouple()
    {
        activeOverseer.DecoupleTrap(currentObjectID, Color.red, GetRoomID());
    }

    public void GameObjectVisible(bool temp) //this controls the visibility
    {
        gameObject.SetActive(temp);
        if (associatedCodeObject1 != null)
        {
            associatedCodeObject1.SetActive(temp);
            associatedCodeObject1.gameObject.SetActive(temp);

        }

        if (associatedCodeObject2 != null)
        {
            associatedCodeObject2.SetActive(temp);
            associatedCodeObject1.gameObject.SetActive(temp);

        }

    }

    public void GameObjectVisibleNetwork() //this controls the visibility
    {
        gameObject.SetActive(trapActive);
        if (associatedCodeObject1 != null)
        {
            associatedCodeObject1.SetActive(trapActive);
            associatedCodeObject1.gameObject.SetActive(trapActive);
        }


        if (associatedCodeObject2 != null)
        {
            associatedCodeObject2.SetActive(trapActive);
            associatedCodeObject1.gameObject.SetActive(trapActive);
        }
        }


    [ClientRpc]
    public void RpcDecoupleTrap() //This handles the UI side of the minigame
    {
        minigameActivated = false;
        if (activeOverseer != null && activeOverseer.GetNumTrap() > 0)
        {
             activeOverseer.DecoupleTrap(currentObjectID, Color.red, GetRoomID());
            //CmdDecouple();
        }

        GameObjectVisible(trapActive);
        CmdSetTrapActive(false);
        //old networking stuff
    }

    public int GetRoomID()
    {
        return currentRoom.roomTag;
    }
}
