using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUIManager : NetworkBehaviour
{
    SyncListBool uiActive = new SyncListBool();
    //bool[] uiActive = new bool[4];
    SyncListInt teamSelected = new SyncListInt();
    SyncListInt roleSelected = new SyncListInt();
    SyncListInt characterSelected = new SyncListInt();
    [SyncVar] public bool updateAllUI; //this is a flag
    [SyncVar] int numInLobby = 0;
    //public int associatedPlayer= 0; //this updates the local ui of the active player!

    public CharacterSelect[] thePlayers = new CharacterSelect[4];
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


    // Use this for initialization
    void Start()
    {

    }

    public void ActivateUI(int index)
    {
        uiActive[index] = true;
        updateAllUI = true;

        UpdateNonLocalUI(index);

        if (isServer)
        {
            CmdLocalUI(index, true);
        }
        else
            localUIs[index].SetActive(true);
    }
    
    public void SetTeam(int index, int number)
    {
        if (isServer)
            CmdSetTeam(index, number);
        else
             teamSelected[index] = number;

        //how you're supposed to do it:
        //if (isServer)
        //{
        //    RpcSetTeam(index, number);
        //}
        //else
        //{
        //    CmdSetTeam(index, number);
        //}
        
    }

    [Command]
    void CmdSetTeam(int index, int number)
    {
        //teamSelected[index] = number;
        RpcSetTeam(index, number);
    }

    [ClientRpc]
    void RpcSetTeam(int index, int number)
    {
        teamSelected[index] = number;
    }


    public void SetRole(int index, int number)
    {
        if (isServer)
            CmdSetRole(index, number);
        else
            roleSelected[index] = number;
    }

    [Command]
    void CmdSetRole(int index, int number)
    {
        RpcSetRole(index, number);
    }

    [ClientRpc]
    void RpcSetRole(int index, int number)
    {
        roleSelected[index] = number;
    }

    public void SetCharacter(int index, int number)
    {
        characterSelected[index] = number;
    }

    [Command]
    void CmdLocalUI(int index, bool active)
    {
        RpcLocalUI(index, active);
    }

    [ClientRpc]
    void RpcLocalUI(int index, bool active)
    {
        localUIs[index].SetActive(active);
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
            if (i == index) thePlayers[i].notLocalPlayer.SetActive(true);
        }
    }

    public int GetCharacter(int index)
    {
        return (int)thePlayers[index].chosenCharacter;
    }

    public int GetRole(int index)
    {
        return thePlayers[index].role;
    }

    public int GetTeam(int index)
    {
        return thePlayers[index].team;
    }

    public bool OnFirstTeam(int index)
    {
        if (thePlayers[index].team == 0) return true;
        return false;
    }

    public bool PlayerReady(int index)
    {
        return thePlayers[index].ready;
    }

    public void LoadProperties()
    {
        uiActive.Add(true); //i was using this to update which 
        uiActive.Add(false);
        uiActive.Add(false);
        uiActive.Add(false);
        
        for (int i = 0; i < 4; i++)
        {
            thePlayers[i].index = i;
            teamSelected.Add(-1);
            roleSelected.Add(-1);
            characterSelected.Add(0);
            //uiActive[i] = false;
        }

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
    }

    public void UpdateUI()
    {
        //  print(uiActive.Count);
        if (updateAllUI && thePlayers[0] != null)
        {
            for (int i = 0; i < uiActive.Count; i++)
            {
                thePlayers[i].gameObject.SetActive(uiActive[i]);
                thePlayers[i].SetLocalActive(uiActive[i]);
                //thePlayers[i].SetBaseActive(!uiActive[i]);

                thePlayers[i].UpdateUI(teamString[teamSelected[i]+1], roleString[roleSelected[i]+1], characterString[characterSelected[i]]);
                thePlayers[i].ChangeImage(characterTextures[characterSelected[i]]);
                //+1 because -1 is used for null
            }
        }
    }
}
