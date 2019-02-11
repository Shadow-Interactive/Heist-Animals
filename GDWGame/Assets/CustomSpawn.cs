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
    
    void Start()
    {
        //prefab choice init
        choice = 0;
        
    }
   
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);

       //Debug.Log("Testing 21321321");

        GameObject ourPlayer;

        if (conn.connectionId == 0)
        {
            //Player 1: They can use the choice variable currently, easier for testing as they are the host
            ourPlayer = Instantiate(spawnPrefabs[choice], startPositions[0].position, Quaternion.identity);

			if (choice == 1)
			{
				ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 1;
				ourPlayer.GetComponent<OverSeerControl>().OverID = 1;
				//print(_temp.GetComponent<OverSeerControl>().OverID);
				print("does get here");
			}
			else
				ourPlayer.GetComponent<PlayerLogic>().runID = 1;

			return ourPlayer;
        }
        else if (conn.connectionId == 1)
        {
            //Player 2: They are coded to be an overseer currently
            ourPlayer = Instantiate(spawnPrefabs[1], startPositions[1].position, Quaternion.identity);

            ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 1;
            ourPlayer.GetComponent<OverSeerControl>().OverID = 1;
            print(ourPlayer.GetComponent<OverSeerControl>().OverID);

            return ourPlayer;
            
        }
        else if (conn.connectionId == 2)
        {
            //Player 3: They are coded to be a runner currently
            ourPlayer = Instantiate(spawnPrefabs[0], startPositions[2].position, Quaternion.identity);

            ourPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material = run2mat;
            ourPlayer.GetComponent<PlayerLogic>().runID = 2;

			return ourPlayer;

        }
        else if (conn.connectionId == 3)
        {
            //Player 4: They are coded to be an overseer currently
            ourPlayer = Instantiate(spawnPrefabs[1], startPositions[3].position, Quaternion.identity);

			ourPlayer.GetComponent<OverseerCanvasManager>().overseerID = 2;
			ourPlayer.GetComponent<OverSeerControl>().OverID = 2;
			print(ourPlayer.GetComponent<OverSeerControl>().OverID);

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
        //uncomment to check if choice is being updated
        //Debug.Log(choice);
            
    }
}
