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
    public GameObject charSelectPrefab;
    public int id;

    public LobbyUIManager theLobbyManager;

    //team 1 overseer then runner positions
    Vector3[] teampositions = new Vector3[4];
    GameObject pleasework;

    public Canvas characterSelect;
    public InputField IPAddress;

    void Start()
    {
        //prefab choice init
        choice = 0;
        theLobbyManager.LoadProperties();
        //  theLobbyManager.ActivateUI(0);
        IPAddress.ActivateInputField();
    }
    //the proper on

    public void ButtonHost()
    { 
        base.StartHost();
    }

    public void ButtonJoin()
    {
        IPAddress = GameObject.Find("InputField").GetComponent<InputField>();
        NetworkLobbyManager.singleton.networkAddress = IPAddress.transform.Find("Text").GetComponent<Text>().text;
        base.StartClient();
    }

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        //theLobbyManager.LoadProperties();
        base.OnLobbyServerConnect(conn);
        base.OnServerAddPlayer(conn, 0); //shortPlayerID 0 means player 1 on a local device I believe? EDIT: after looking it up, playerControllerID enables you to have multiple players on a client, so we can leave it as 0
        //so because we have no local multiplayer this works
        Debug.Log(conn.connectionId);
        id = conn.connectionId;
    }

    //might need OnLobbyClientConnect
    
    //this one is for the host
    public void ButtonDisconnect()
    {
        //stop host AND server
        base.StopHost();
        NetworkLobbyManager.singleton.StopServer();
        //NetworkServer.DisconnectAll();

        //NetworkServer.Ho
    }

    //this one is for the clients
    public void ButtonDisconnectClient()
    {
        //stop client
        base.StopClient();
    }

    //public override void OnLobbyStopHost()
    //{
    //    print("There is no longer a host");
    //    base.OnLobbyStopHost();
    //}

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        print("Goodbye: " + conn);
        base.OnLobbyClientDisconnect(conn);
    }
    //public override void OnServerConnect(NetworkConnection conn)
    //{
    //    base.OnServerConnect(conn);
    //    base.OnServerAddPlayer(conn, 0);
    //}

    //public override void OnClientConnect(NetworkConnection conn)
    //{
    //    base.OnClientConnect(conn);
    //    base.OnServerAddPlayer(conn, 0);
    //}

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("When does this get called");

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
            CharacterSelect player1 = GameObject.Find("Player0").GetComponent<CharacterSelect>();
            
            int prefabChoice = player1.role;
            int teamChoice;
            if (player1.team == 0) teamChoice = 0;
            else teamChoice = 2;
            
            int team = player1.team +1;
            
             ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice+teamChoice].position, Quaternion.identity);
            
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
                 ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter((int)player1.chosenCharacter);
             }
             
            player1.DisableCharacterSelect();
            player1.inCharacterSelect = false;
            
            if (GameObject.Find("Player1"))
            {
                GameObject.Find("Player1").GetComponent<CharacterSelect>().inCharacterSelect = false;
                GameObject.Find("Player1").GetComponent<CharacterSelect>().DisableCharacterSelect();
            
            }
            if (GameObject.Find("Player2"))
            {
                GameObject.Find("Player2").GetComponent<CharacterSelect>().inCharacterSelect = false;
                GameObject.Find("Player2").GetComponent<CharacterSelect>().DisableCharacterSelect();
            }
            if (GameObject.Find("Player3"))
            {
                GameObject.Find("Player3").GetComponent<CharacterSelect>().inCharacterSelect = false;
                GameObject.Find("Player3").GetComponent<CharacterSelect>().DisableCharacterSelect();
            }
            
           // ourPlayer = Instantiate(spawnPrefabs[1], startPositions[0].position, Quaternion.identity);
           // ourPlayer.GetComponent<PlayerLogic>().runID = 0;
            return ourPlayer;
        }
        else if (conn.connectionId == 1)
        {
            CharacterSelect player2 = GameObject.Find("Player1").GetComponent<CharacterSelect>();
            
            int prefabChoice = player2.role;
            int teamChoice;
            if (player2.team == 0) teamChoice = 0;
            else teamChoice = 2;
            
            int team = player2.team+1;
            
            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice+teamChoice].position, Quaternion.identity);
            
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
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter((int)player2.chosenCharacter);
            }
            
            player2.DisableCharacterSelect();
            player2.inCharacterSelect = false;
            
            //ourPlayer = Instantiate(spawnPrefabs[1], startPositions[1].position, Quaternion.identity);
            //ourPlayer.GetComponent<OverseerCanvasManager>().overseerID =0;
            //ourPlayer.GetComponent<OverSeerControl>().OverID = 0;
            return ourPlayer;

        }
        else if (conn.connectionId == 2)
        {
            CharacterSelect player3 = GameObject.Find("Player2").GetComponent<CharacterSelect>();

            int prefabChoice = player3.role;
            int teamChoice;
            if (player3.team == 0) teamChoice = 0;
            else teamChoice = 2;
            
            int team = player3.team + 1;
            
            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice+teamChoice].position, Quaternion.identity);
            
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
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter((int)player3.chosenCharacter);
            }
           
            player3.DisableCharacterSelect();
            player3.inCharacterSelect = false;
            
            //   ourPlayer = Instantiate(spawnPrefabs[0], startPositions[2].position, Quaternion.identity);
            //ourPlayer.GetComponent<PlayerLogic>().runID = 2;
            return ourPlayer;

        }
        else if (conn.connectionId == 3)
        {
            CharacterSelect player4 = GameObject.Find("Player3").GetComponent<CharacterSelect>();

            int prefabChoice = player4.role;
            int teamChoice;
            if (player4.team == 0) teamChoice = 0;
            else teamChoice = 2;
            
            int team = player4.team + 1;
            
            ourPlayer = Instantiate(spawnPrefabs[prefabChoice], startPositions[prefabChoice+teamChoice].position, Quaternion.identity);
            
            if (prefabChoice == 1) //1 for overseer
            {
                ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = team;
                ourPlayer.GetComponent<OverSeerControl>().OverID = team;
                print("does get here");
            }
            else
            {
                ourPlayer.GetComponent<PlayerLogic>().runID = team;
                ourPlayer.GetComponent<PlayerLogic>().SetChosenCharacter((int)player4.chosenCharacter);
            }
            
            player4.DisableCharacterSelect();
            player4.inCharacterSelect = false;
            
           //ourPlayer = Instantiate(spawnPrefabs[1], startPositions[3].position, Quaternion.identity);
           //ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 2;
           //ourPlayer.GetComponent<OverSeerControl>().OverID = 2;
            return ourPlayer;

        }
        else return null;
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
        if (theLobbyManager != null)
            theLobbyManager.UpdateUI();
        else if (GameObject.FindGameObjectsWithTag("Runner") != null)
            theLobbyManager = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>();

        if (GameObject.FindGameObjectWithTag("Runner") == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
