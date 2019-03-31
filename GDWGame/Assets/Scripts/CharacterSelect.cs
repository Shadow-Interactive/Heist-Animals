﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum Characters
{
    nocharacter = -1,
    character1 = 1,
    character2 = 2,
    character3 = 3, 
    character4 = 4
}

public class CharacterSelect : NetworkBehaviour {

    int nullNumber = -1;
    [HideInInspector] [SyncVar] public int team = -1; //Team 1 or Team 2? //choice0
    [HideInInspector] [SyncVar] public int role = -1; //Overseer or Runner? //choice1
    [HideInInspector] [SyncVar] public Characters chosenCharacter = Characters.character1; //choice2
    [HideInInspector] public int currentChoice = 0; //so that we can have a back button 
    [HideInInspector] [SyncVar] public bool ready = false; //this is to say that they're reaedy to start the actual game
    [HideInInspector] [SyncVar] public int index = 0;
    [HideInInspector] [SyncVar] public bool inCharacterSelect = true;

    //for the specific buttons //the other two options are just buttons
    int characterCounter = 0; //this is to switch between the possible characters

    //UI variables (this would probably just be used to set colours and stuff?

    //the Team member
    //to make sure that there aren't 2 runners or anything.
    CharacterSelect partner;

    //to update all the ui 
    bool updateUI = false;

    //other ui stuff
    public RawImage theCharacter, background, imgNullRole;
    public GameObject notLocalPlayer, localPlayer, currentPlayer;
    public RawImage[] tutorials = new RawImage[2];
    //Color[] theCharacterColors = new Color[4];

    public GameObject teamChosen, roleChosen, roleObject, characterObject, teamObject, buttonObjects, charLocal;
    public Text teamText, roleText, chosenTeamText, chosenRoleText, chosenCharacterText, overseerText;

    //logic variables
    public LobbyUIManager theLobbyManager; //im praying to god that this works
    public CustomSpawn connectionID;
    [SyncVar] bool loadProperties = false;
    [SyncVar] bool setPositions = false;
    [SyncVar] Vector3 uiPosition = new Vector3(0, 0, 0);
    [SyncVar] string gameobjectName = "Player";
    [SyncVar] int playerIndex = 0;

    //stupid proofing
    public Button btnReady;

    // Use this for initialization
    void Start() {
        btnReady.gameObject.SetActive(false); //wouldn't be active to start with

        if (!isLocalPlayer)
        {
            //will clean this up later
            roleObject.gameObject.SetActive(false);
            charLocal.gameObject.SetActive(false);
            teamObject.gameObject.SetActive(false);
            buttonObjects.SetActive(false);
            currentPlayer.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {

        if (inCharacterSelect)
        {
            if (theLobbyManager == null)
            {
                if (GameObject.Find("LobbyUIManager"))
                    theLobbyManager = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>();

                if (GameObject.Find("LobbyManager"))
                    connectionID = GameObject.Find("LobbyManager").GetComponent<CustomSpawn>();
            }
            else
            {
                HowDoesThisWork();
                chosenTeamText.text = theLobbyManager.teamString[team + 1];
                chosenRoleText.text = theLobbyManager.roleString[role + 1];
                chosenCharacterText.text = theLobbyManager.characterString[(int)chosenCharacter];

                Cursor.lockState = CursorLockMode.None;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

            }

            SettingPositions();

            gameObject.name = gameobjectName;
            notLocalPlayer.transform.position = uiPosition;
            localPlayer.transform.position = uiPosition;

        }
        else
        {
            inCharacterSelect = false;
            notLocalPlayer.SetActive(false);
            localPlayer.SetActive(false);
        }

        //print(chosenTeamText.text);
    }

    void SettingPositions()
    {
        if (!setPositions && hasAuthority)
        {
            if (isServer)
                RpcSettingPositions();
            else
                CmdSettingPositions();
        }
    }

    public void DisableCharacterSelect()
    {
        inCharacterSelect = false;
        notLocalPlayer.SetActive(false);
        localPlayer.SetActive(false);
        if (isServer)
            RpcSettingPositions();
        else
            CmdSettingPositions();
        //gameObject.SetActive(false);
    }

    [Command]
    void CmdDisableCharacterSelect()
    {
        inCharacterSelect = false;
        notLocalPlayer.SetActive(false);
        localPlayer.SetActive(false);
        RpcDisableCharacterSelect();
    }

    [ClientRpc]
    void RpcDisableCharacterSelect()
    {
        inCharacterSelect = false;
        notLocalPlayer.SetActive(false);
        localPlayer.SetActive(false);
    }

    [Command]
    void CmdSettingPositions()
    {
        RpcSettingPositions();
    }

    [ClientRpc]
    void RpcSettingPositions()
    {
        if (theLobbyManager != null)
        {
            notLocalPlayer.transform.position = theLobbyManager.uiPositions[playerIndex];
            localPlayer.transform.position = theLobbyManager.uiPositions[playerIndex];
            uiPosition = theLobbyManager.uiPositions[playerIndex];
            setPositions = true;
        }
    }

    void HowDoesThisWork()
    {
        if (!loadProperties && hasAuthority)
        {
            if (isServer)
                RpcUpdateCharacter();
            else
                CmdUpdateCharacter();
        }
    }

    [Command]
    void CmdUpdateCharacter()
    {
        RpcUpdateCharacter();
    }

    [ClientRpc]
    void RpcUpdateCharacter()
    {
        //print(theLobbyManager.playerNumbers.Count);
        for (int i = 0; i < 4; i++)
        {
            if (theLobbyManager != null)
            {
                if (loadProperties == false && theLobbyManager.playerNumbers[connectionID.id] == -1)
                {
                    theLobbyManager.playerNumbers[i] = i;
                    //THis line is so important
                    gameobjectName = "Player" + connectionID.id;
                    playerIndex = connectionID.id;
                    loadProperties = true;
                }
            }
        }

    }


    public void ChooseTeam() //in the button you'd set the variable to 1 or 2 to indication which team it is
    {
        if (isServer)
        {
            RpcUpdateTeam();
        }
        else
        {
            CmdUpdateTeam();
        }
    }

    [Command]
    void CmdUpdateTeam()
    {
        RpcUpdateTeam();
    }

    [ClientRpc]
    void RpcUpdateTeam()
    {
        ChooseTeamLogic();
    }

    void ChooseTeamLogic()
    {
        //this makes sure that the team chosen won't revert back to null
        int desiredTeam = team;
        int newteam = 0;
        if (desiredTeam == nullNumber) desiredTeam = 0;
        else desiredTeam = desiredTeam == 0 ? 1 : 0;
        newteam = theLobbyManager.AvailableTeam(playerIndex, desiredTeam);
        ThisIsADisasterTeam(newteam);
        SetTeamUI();

        if (role != nullNumber)
        {
            //if they picked the role first and then the team
            //then it sees what role is available and switches them to it
            int newRole = theLobbyManager.AvailableRole(playerIndex, team, role);
            SetRole(newRole);
        }
    }

    void ThisIsADisasterTeam(int num)
    {
        if (isServer) RpcTeam(num);
        else CmdTeam(num);        
    }

    [Command]
    void CmdTeam(int num)
    {
        RpcTeam(num);
    }

    [ClientRpc]
    void RpcTeam(int num)
    {
        team = num;
        CheckIfReady();
    }

    void SetTeamUI()
    {
        if (isServer) RpcTeamUILogic();
        else CmdTeamUILogic();
    }

    [Command]
    void CmdTeamUILogic()
    {
        RpcTeamUILogic();
    }

    [ClientRpc]
    void RpcTeamUILogic()
    {
        if (theLobbyManager != null)
        {
            if (team == 0)
            {
                background.texture = theLobbyManager.team1Texture;
            }
            else if (team == 1)
            {
                background.texture = theLobbyManager.team2Texture;

            }
        }
    }
    
    public void ChooseRole() //in the button you'd set the variable to 0 or 1 to indicate the role it is
    {
        if (isServer)
            RpcUpdateRole();
        else
            CmdUpdateRole();
    }

    [Command]
    void CmdUpdateRole()
    {
        RpcUpdateRole();
    }

    [ClientRpc]
    void RpcUpdateRole()
    {
        ChooseRoleLogic();
    }

    void ChooseRoleLogic()
    {
        imgNullRole.gameObject.SetActive(false); //shows the role stuff
        int desiredRole = role;
        int newRole = 0;
        if (desiredRole == nullNumber) desiredRole = 0;
        else desiredRole = desiredRole == 0 ? 1 : 0;

        newRole = theLobbyManager.AvailableRole(playerIndex, team, desiredRole);
        SetRole(newRole);
        
    }

    public void CharacterIncrementerRight()
    {
        if (isServer)
            RpcCharacterIncrementerRight();
        else
            CmdCharacterIncrementerRight();
    }

    [Command]
    void CmdCharacterIncrementerRight()
    {
        RpcCharacterIncrementerRight();
    }

    [ClientRpc]
    void RpcCharacterIncrementerRight()
    {
        RightIncrementLogic();
    }

    void RightIncrementLogic()
    {
        characterCounter++;

        if (characterCounter >= 2)
        {
            characterCounter = 0;
        }

        UpdateCharacter();
    }

    public void CharacterIncrementerLeft()
    {
        if (isServer)
            RpcCharacterIncrementerLeft();
        else
            CmdCharacterIncrementerLeft();
    }

    [Command]
    void CmdCharacterIncrementerLeft()
    {
        RpcCharacterIncrementerLeft();
    }

    [ClientRpc]
    void RpcCharacterIncrementerLeft()
    {
        LeftIncrementLogic();
    }

    void LeftIncrementLogic()
    {
        characterCounter--;

        if (characterCounter < 0)
        {
            characterCounter = 1;
        }

        UpdateCharacter();
    }

    void UpdateCharacter()
    {
        chosenCharacter = (Characters)characterCounter;
        if (theLobbyManager != null)
            theCharacter.texture = theLobbyManager.characterTextures[(int)chosenCharacter];
    }

    public void SetLocalActive(bool active)
    {
        localPlayer.SetActive(active);
    }

    public void SetBaseActive(bool active)
    {
        notLocalPlayer.SetActive(active);
    }

    public void UpdateUI(string chosenTeam, string chosenRole, string chosenCharacter)
    {
        chosenTeamText.text = chosenTeam;
        chosenRoleText.text = chosenRole;
        chosenCharacterText.text = chosenCharacter;

        SetRoleUI();
        SetTeamUI();
    }

    public void ChangeImage(Texture newImage)
    {
        theCharacter.texture = newImage;
    }

    public void Tutorials()
    {
        if (role != nullNumber)
        {
            int whichTutorial = role;
            tutorials[role].gameObject.SetActive(!tutorials[role].IsActive());

        }
    }

    public void ClearTeam()
    {
        if (isServer) RpcClearTeam();  
        else CmdClearTeam();       
    }

    [Command]
    void CmdClearTeam()
    {
        RpcClearTeam();
    }

    [ClientRpc]
    void RpcClearTeam()
    {
        team = nullNumber;
        btnReady.gameObject.SetActive(false); //it's setting it to null so automatically ready should disappear
    }

    public void ClearRole()
    {
        //calls the right function depending on the server       
        if (isServer) RpcClearRole();        
        else CmdClearRole();       
    }

    [Command]
    void CmdClearRole()
    {
        RpcClearRole();
    }

    [ClientRpc]
    void RpcClearRole()
    {
        role = nullNumber;
        btnReady.gameObject.SetActive(false); //it's setting it to null so automatically ready should disappear
        imgNullRole.gameObject.SetActive(true); //shows the role stuff
    }

    public void SetRole(int newRole)
    {
        if (isServer) RpcSetRole(newRole);      
        else CmdSetRole(newRole);
    }

    [Command]
    public void CmdSetRole(int newRole)
    {
        RpcSetRole(newRole);
    }

    [ClientRpc]
    public void RpcSetRole(int newRole)
    {
        role = newRole;
        SetRoleUI();
        CheckIfReady();
    }
    
    void SetRoleUI()
    {
        if (isServer) RpcSetRoleUI();
        else CmdSetRoleUI();
    }

    [Command]
    void CmdSetRoleUI()
    {
        RpcSetRoleUI();
    }

    [ClientRpc]
    void RpcSetRoleUI()
    {
        if (role == 1) //runner
        {
            RoleUILogic(true);
        }
        else if (role == 0)//overseer
        {
            RoleUILogic(false);
        }
    }

    void RoleUILogic(bool active)
    {
        characterObject.SetActive(active);
        theCharacter.gameObject.SetActive(active);
        chosenCharacterText.gameObject.SetActive(active);
        overseerText.gameObject.SetActive(!active);

        if (isServer) RpcNullRole(false);
        else CmdNullRole(false);
    }

    [Command]
    void CmdNullRole(bool active)
    {
        RpcNullRole(active);
    }

    [ClientRpc]
    void RpcNullRole(bool active)
    {
        imgNullRole.gameObject.SetActive(active);
    }

    void CheckIfReady()
    {
        if (team == nullNumber || role == nullNumber)
        {
            btnReady.gameObject.SetActive(false);
        }
        else
            btnReady.gameObject.SetActive(true);
    }

    void CheckRole()
    {

    }
}
