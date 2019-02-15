using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CustomSpawn : NetworkLobbyManager
{
    //these might not be needed anymore, leaving them for now
    public Movement runner;
    public OverSeerControl overseer;

    //spawn position
    public Transform Spawn;

    //prefab int
    public int choice;
    public Material run2mat;

    public LobbyUIManager theLobbyManager;

    //team 1 overseer then runner positions
    Vector3[] teampositions = new Vector3[4];


    void Start()
    {
        //prefab choice init
        choice = 0;
        theLobbyManager.LoadProperties();
        theLobbyManager.ActivateUI(0);

    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);

        teampositions[0] = startPositions[0].position; //runner1
        teampositions[1] = startPositions[1].position; //overseer1
        teampositions[2] = startPositions[2].position; //runner2
        teampositions[3] = startPositions[3].position; //overseer2

        //Debug.Log("Testing 21321321");

        GameObject ourPlayer;

        if (conn.connectionId == 0)
        {
            //Player 1: They can use the choice variable currently, easier for testing as they are the host
            //ourPlayer = Instantiate(spawnPrefabs[choice], startPositions[0].position, Quaternion.identity);

            int prefabChoice = theLobbyManager.GetRole(0);
            int teamChoice = theLobbyManager.OnFirstTeam(0) ? 0 : 2;
            int team = theLobbyManager.GetTeam(0) + 1;

            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice + teamChoice].position, Quaternion.identity);

            if (prefabChoice == 1) //1 for overseer
            {
                ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = team;
                ourPlayer.GetComponent<OverSeerControl>().OverID = team;
                //print(_temp.GetComponent<OverSeerControl>().OverID);
                print("does get here");
            }
            else
            {
                ourPlayer.GetComponent<PlayerLogic>().runID = team;
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter(theLobbyManager.GetCharacter(0));
            }
            theLobbyManager.updateAllUI = false;

            return ourPlayer;
        }
        else if (conn.connectionId == 1)
        {
            //Player 2: They are coded to be an overseer currently
            //ourPlayer = Instantiate(spawnPrefabs[1], startPositions[1].position, Quaternion.identity);
            int prefabChoice = theLobbyManager.GetRole(1);
            int teamChoice = theLobbyManager.OnFirstTeam(1) ? 0 : 2;
            int team = theLobbyManager.GetTeam(1) + 1;

            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice + teamChoice].position, Quaternion.identity);

            if (prefabChoice == 1)
            {
                ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = team;
                ourPlayer.GetComponent<OverSeerControl>().OverID = team;
                //print(_temp.GetComponent<OverSeerControl>().OverID);
                print("does get here");
            }
            else
            {
                ourPlayer.GetComponent<PlayerLogic>().runID = team;
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter(theLobbyManager.GetCharacter(1));
            }

            //ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 1;
            //ourPlayer.GetComponent<OverSeerControl>().OverID = 1;
            //print(ourPlayer.GetComponent<OverSeerControl>().OverID);
            theLobbyManager.updateAllUI = false;

            return ourPlayer;

        }
        else if (conn.connectionId == 2)
        {
            //Player 3: They are coded to be a runner currently
            //ourPlayer = Instantiate(spawnPrefabs[0], startPositions[2].position, Quaternion.identity);
            //
            //ourPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material = run2mat;
            //ourPlayer.GetComponent<PlayerLogic>().runID = 2;
            //ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter(theLobbyManager.GetCharacter(2));

            int prefabChoice = theLobbyManager.GetRole(2);
            int teamChoice = theLobbyManager.OnFirstTeam(2) ? 0 : 2;
            int team = theLobbyManager.GetTeam(2) + 1;

            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice + teamChoice].position, Quaternion.identity);

            if (prefabChoice == 1)
            {
                ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = team;
                ourPlayer.GetComponent<OverSeerControl>().OverID = team;
                //print(_temp.GetComponent<OverSeerControl>().OverID);
                print("does get here");
            }
            else
            {
                ourPlayer.GetComponent<PlayerLogic>().runID = team;
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter(theLobbyManager.GetCharacter(2));
            }
            theLobbyManager.updateAllUI = false;

            return ourPlayer;

        }
        else if (conn.connectionId == 3)
        {
            //Player 4: They are coded to be an overseer currently
            //ourPlayer = Instantiate(spawnPrefabs[1], startPositions[3].position, Quaternion.identity);
            //
            //ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 2;
            //ourPlayer.GetComponent<OverSeerControl>().OverID = 2;
            //print(ourPlayer.GetComponent<OverSeerControl>().OverID);

            int prefabChoice = theLobbyManager.GetRole(3);
            int teamChoice = theLobbyManager.OnFirstTeam(3) ? 0 : 2;
            int team = theLobbyManager.GetTeam(3) + 1;

            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice + teamChoice].position, Quaternion.identity);

            if (prefabChoice == 1)
            {
                ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = team;
                ourPlayer.GetComponent<OverSeerControl>().OverID = team;
                //print(_temp.GetComponent<OverSeerControl>().OverID);
                print("does get here");
            }
            else
            {
                ourPlayer.GetComponent<PlayerLogic>().runID = team;
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter(theLobbyManager.GetCharacter(3));
            }
            theLobbyManager.updateAllUI = false;

            return ourPlayer;

        }
        else return null;
    }

    public override void OnServerConnect(NetworkConnection player)
    {
        if (player.hostId >= 0)
        {
            theLobbyManager.ActivateUI(player.connectionId);

            //  theLobbyManager.playerUIs[player.connectionId].SetActive(true);
        }
    }

    public void setRunnerRole()
    {
        //set prefab choice to 0
        choice = 0;

    }

    public void setOverseerRole()
    {
        //set prefab choice to 1
        choice = 1;
    }

    void Update()
    {
        //uncomment to check if choice is being updated
        //Debug.Log(choice);
        theLobbyManager.UpdateUI();
        //if(Input.GetKey(KeyCode.D))
        //{
        //    theLobbyManager.ActivateUI();
        //}
        //
    }
}
