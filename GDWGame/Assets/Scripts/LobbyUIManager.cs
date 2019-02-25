using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUIManager : NetworkBehaviour
{
    public SyncListBool uiActive = new SyncListBool();
    //bool[] uiActive = new bool[4];
  //SyncListInt teamSelected = new SyncListInt();
  //SyncListInt roleSelected = new SyncListInt();
  //SyncListInt characterSelected = new SyncListInt();
    [SyncVar] public bool updateAllUI; //this is a flag
    [SyncVar] int numInLobby = 0;
    //public int associatedPlayer= 0; //this updates the local ui of the active player!

   // public CharacterSelect[] thePlayers = new CharacterSelect[4];
    public GameObject[] localUIs = new GameObject[4]; //this is a hacky solution
    public GameObject[] nonLocalUIs = new GameObject[4];
    public Texture[] characterTextures = new Texture[4];

    //this is to fix syncing?
    public Canvas playerCanvas;
    //to bugtest
    public Text uiTest;

    //the text to appear on screen
    string[] teamString = new string[3];
    string[] roleString = new string[3];
    string[] characterString = new string[4];

    public CharacterSelect[] actualPlayers = new CharacterSelect[4];
    public Vector3[] uiPositions = new Vector3[4];

    public CharacterSelect oldPlayer;

    public SyncListInt playerNumbers = new SyncListInt();
    [SyncVar] bool locatingPlayers = true;

    // Use this for initialization
    void Start()
    {

    }

    [Command]
    void CmdUpdateUINonPlayer(int index)
    {
        RpcUpdateUINonPlayer(index);
    }

    [ClientRpc]
    void RpcUpdateUINonPlayer(int index)
    {
        UpdateNonLocalUI(index);
    }

    void UpdateNonLocalUI(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == index) actualPlayers[i].notLocalPlayer.SetActive(true);
        }
    }

    public int GetCharacter(int index)
    {
        return (int)actualPlayers[index].chosenCharacter;
    }

    public int GetRole(int index)
    {
        return actualPlayers[index].role;
    }

    public int GetTeam(int index)
    {
        return actualPlayers[index].team;
    }

    public bool OnFirstTeam(int index)
    {
        if (actualPlayers[index].team == 0) return true;
        return false;
    }

    public bool PlayerReady(int index)
    {
        return actualPlayers[index].ready;
    }

    public void LoadProperties()
    {
        uiActive.Add(true); //i was using this to update which 
        uiActive.Add(false);
        uiActive.Add(false);
        uiActive.Add(false);

        teamString[0] = "N/A";
        teamString[1] = "Team 1";
        teamString[2] = "Team 2";

        roleString[0] = "N/A";
        roleString[1] = "Runner";
        roleString[2] = "Overseer";

        characterString[0] = "Monkey";
        characterString[1] = "Chicken";
        characterString[2] = "Cat";
        characterString[3] = "Diamond";

        playerNumbers.Add(-1);
        playerNumbers.Add(-1);
        playerNumbers.Add(-1);
        playerNumbers.Add(-1);

    }

    public void LocatingPlayers()
    {
        if (!locatingPlayers)
        {
            if (isServer)
                RpcLocatingPlayers();
            else
                CmdLocatingPlayers();
        }
    }

    [Command]
    void CmdLocatingPlayers()
    {
        RpcLocatingPlayers();
    }

    [ClientRpc]
    void RpcLocatingPlayers()
    {
        if (GameObject.Find("Player0"))
            actualPlayers[0] = GameObject.Find("Player0").GetComponent<CharacterSelect>();

        if (GameObject.Find("Player1"))
            actualPlayers[1] = GameObject.Find("Player1").GetComponent<CharacterSelect>();

        if (GameObject.Find("Player2"))
            actualPlayers[2] = GameObject.Find("Player2").GetComponent<CharacterSelect>();

        if (GameObject.Find("Player3"))
        {
            actualPlayers[3] = GameObject.Find("Player3").GetComponent<CharacterSelect>();
            locatingPlayers = false;
        }
    }

    public void UpdateUI()
    {
        LocatingPlayers();

        if (oldPlayer != null)
        {
            oldPlayer.gameObject.SetActive(uiActive[0]);
            oldPlayer.SetLocalActive(uiActive[0]);
            // actualPlayer.UpdateUI(teamString[teamSelected[0] + 1], roleString[roleSelected[0] + 1], characterString[characterSelected[0]]);
          //  oldPlayer.ChangeImage(characterTextures[characterSelected[0]]);
        }

        for (int i = 0; i < 4; i++)
        {
            if (actualPlayers[i] != null)
            {
                //actualPlayers[i].gameObject.SetActive(uiActive[i]);
                //actualPlayers[i].SetLocalActive(uiActive[i]);
                // actualPlayer.UpdateUI(teamString[teamSelected[0] + 1], roleString[roleSelected[0] + 1], characterString[characterSelected[0]]);
             //   actualPlayers[i].ChangeImage(characterTextures[characterSelected[i]]);
            }
        }
    }
}
