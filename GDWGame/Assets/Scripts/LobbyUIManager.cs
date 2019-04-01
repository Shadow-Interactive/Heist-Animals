using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUIManager : NetworkBehaviour
{
    public SyncListBool uiActive = new SyncListBool();
    [SyncVar] public bool updateAllUI; //this is a flag
    [SyncVar] int numInLobby = 0;
    
    public GameObject[] localUIs = new GameObject[4]; //this is a hacky solution
    public GameObject[] nonLocalUIs = new GameObject[4];
    public Texture[] characterTextures = new Texture[4];

    public Texture team1Texture, team2Texture;

    //this is to fix syncing?
    public Canvas playerCanvas;
    //to bugtest
    public Text uiTest;

    //the text to appear on screen
    [HideInInspector] public string[] teamString = new string[3];
    [HideInInspector] public string[] roleString = new string[3];
    [HideInInspector] public string[] characterString = new string[4];

    public CharacterSelect[] actualPlayers = new CharacterSelect[4];
    public Vector3[] uiPositions = new Vector3[4];

    public SyncListInt playerNumbers = new SyncListInt();
    bool locatingPlayers = true;

    // Use this for initialization
    void Start()
    {
        //this info is in the start function too so that it reloads when someone disconnects then reconnects
        //it's because the lobbymanager never unloads so the loadproperties function is only ever called once

        uiActive.Add(true);
        uiActive.Add(false);
        uiActive.Add(false);
        uiActive.Add(false);

        teamString[0] = "N/A";
        teamString[1] = "Team 1";
        teamString[2] = "Team 2";

        roleString[0] = "N/A";
        roleString[2] = "Runner";
        roleString[1] = "Overseer";

        characterString[0] = "Tiger";
        characterString[3] = "Tuesday";
        characterString[1] = "Shadow";
        characterString[2] = "Doug";

        playerNumbers.Add(-1);
        playerNumbers.Add(-1);
        playerNumbers.Add(-1);
        playerNumbers.Add(-1);
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
        if (locatingPlayers)
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
            }
        }
    }

    public void UpdateUI()
    {
        if (playerNumbers[0] != -1)
        LocatingPlayers();
    }

    //you use this to go thru and see if a team has 2 players or not
    public int AvailableTeam(int index, int desiredTeam)
    {
        int team0 = 0, team1 = 0;
        for (int i = 0; i < 4; i++)
        {
            if (actualPlayers[i] != null && i != index)
            {
                if (actualPlayers[i].team == 0) team0++;
                else if (actualPlayers[i].team == 1) team1++;
            }
        }

        if (desiredTeam == 0)
        {
            if (team0 < 2) return 0;
            else return 1;
        }
        else
        {
            if (team1 < 2) return 1;
            else return 0;
        }
    }

    //you use this to go thru and see if a team has 2 players or not
    public int AvailableRole(int index, int team, int desiredRole)
    {
        //i know this is bad but pls forgive me
        //im rushing just to get this done
        for (int i = 0; i < 4; i++)
        {
            //checks the player, and if it's not the index
            if (actualPlayers[i] != null && i != index && team != -1 && actualPlayers[i].team == team) //if its not null, not the player calling the function, and the same team
            {
                if(actualPlayers[i].role == desiredRole) //if that player already has that role
                {
                    if (desiredRole == 0)
                        actualPlayers[i].SetRole(1);  
                    else
                        actualPlayers[i].SetRole(0);
                }
            }
        }

        return desiredRole;
    }

}
