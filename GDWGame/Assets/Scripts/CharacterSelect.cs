using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum Characters
{
    nocharacter = -1,
    character1 = 0,
    character2 = 1,
    character3 = 2, 
    character4 = 3
}

public class CharacterSelect : NetworkBehaviour {

    int nullNumber = -1;
    [HideInInspector] [SyncVar] public int team = -1; //Team 1 or Team 2? //choice0
    [HideInInspector] [SyncVar] public int role = -1; //Overseer or Runner? //choice1
    [HideInInspector] [SyncVar] public Characters chosenCharacter = Characters.character1; //choice2
    [HideInInspector] public int currentChoice = 0; //so that we can have a back button 
    [HideInInspector] [SyncVar] public bool ready = false; //this is to say that they're reaedy to start the actual game
    [HideInInspector] [SyncVar] public int index = 0;

    //for the specific buttons //the other two options are just buttons
    int characterCounter = 0; //this is to switch between the possible characters

    //UI variables (this would probably just be used to set colours and stuff?

    //the Team member
    //to make sure that there aren't 2 runners or anything.
    CharacterSelect partner;

    //to update all the ui 
    bool updateUI = false;

    //other ui stuff
    public RawImage theCharacter;
    public GameObject notLocalPlayer, localPlayer;
    public RawImage[] tutorials = new RawImage[2];
    //Color[] theCharacterColors = new Color[4];

    public GameObject teamChosen, roleChosen, roleObject, characterObject;
    public Text teamText, roleText, chosenTeamText, chosenRoleText, chosenCharacterText;

    public LobbyUIManager theLobbyManager; //im praying to god that this works

	// Use this for initialization
	void Start () {
         
    }

    // Update is called once per frame
    void Update () {
    }
    
    //
    public void ChooseTeam() //in the button you'd set the variable to 1 or 2 to indication which team it is
    {
        if (isServer)
            CmdUpdateTeam();
        else
            ChooseTeamLogic();

       //if (this.isLocalPlayer == true) { return; }
       //
       //// the way you're supposed to do it:
       //if (isServer)
       //    {
       //        RpcUpdateTeam();
       //    }
       //    else
       //    {
       //        CmdUpdateTeam();
       //    }
    }

    [Command]
    void CmdUpdateTeam()
    {
        //RpcUpdateTeam();
        ChooseTeamLogic();
    }

    [ClientRpc]
    void RpcUpdateTeam()
    {
        //theLobbyManager.SetTeam(index, num);
        ChooseTeamLogic();
    }

    void ChooseTeamLogic()
    {
        //this makes sure that the team chosen won't revert back to null
        if (team == nullNumber) team = 0;
        else team = team == 0 ? 1 : 0;
        theLobbyManager.SetTeam(index, team);
    }

    public void ChooseRole() //in the button you'd set the variable to 0 or 1 to indicate the role it is
    {
        if (isServer )
           CmdUpdateRole();
        else
            ChooseRoleLogic();

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
        if (role == nullNumber) role = 0;
        else role = role == 0 ? 1 : 0;

        //if (partner != null)
            //partner.role = ( == 0 ? 1 : 0); //ternery operators amirite
        
        theLobbyManager.SetRole(index, role);
        //Advance();
    }

    public void CharacterIncrementerRight()
    {
        if (isServer)
            CmdCharacterIncrementerRight();
        else
            RightIncrementLogic();
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

        if (characterCounter >= 4)
        {
            characterCounter = 0;
        }

        UpdateCharacter();
    }

    public void CharacterIncrementerLeft()
    {
      if (isServer)
          CmdCharacterIncrementerLeft();
      else
            LeftIncrementLogic();
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
            characterCounter = 3;
        }

        UpdateCharacter();
    }

    void UpdateCharacter()
    {
        chosenCharacter = (Characters)characterCounter;
        //theCharacter.color = theCharacterColors[characterCounter];
        //chosenCharacterText.text = chosenCharacter.ToString();
        theLobbyManager.SetCharacter(index, (int)chosenCharacter);
    }

    //not using back or advance
    public void Back()
    {
        if (currentChoice > 0) currentChoice--;
        ready = false;
        switch (currentChoice)
        {
            case 0:
                team = nullNumber;
                theLobbyManager.SetTeam(index, nullNumber);
                teamChosen.SetActive(false);
                roleObject.SetActive(false);
                //this does nothing lol 
                break;
            case 1:
                role = nullNumber;
                roleChosen.SetActive(false);
                theLobbyManager.SetRole(index, nullNumber);

                characterCounter = 0;
                chosenCharacter = 0;
                theLobbyManager.SetCharacter(index, 0);
                theCharacter.color = Color.green;
                characterObject.SetActive(false);
                break;
            case 2:
                
                break;
            case 3:
                
                break;
        }
        
    }

    //not using back or advance
    public void Advance()
    {
        if (currentChoice < 3) currentChoice++;

        switch(currentChoice)
        {
            case 0:
            //this will never be 0 tho lol
                break;
            case 1:
                //teamChosen.SetActive(true);
                //teamText.text = "Team: " + team;

                roleObject.SetActive(true);
                break;
            case 2:
                //roleChosen.SetActive(true);
                //roleText.text = "Role: " + role;
                //theCharacter.color = Color.green;
                characterObject.SetActive(true);
                ready = true;
                break;
            case 3:

                break;
        }
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
}
