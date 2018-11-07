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

        GameObject _temp;

        if (conn.connectionId == 0)
        {
            //Player 1: They can use the choice variable currently, easier for testing as they are the host
            _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[choice],
                startPositions[conn.connectionId].position,
                Quaternion.identity);

            return _temp;
        }
        else if (conn.connectionId == 1)
        {
            //Player 2: They are coded to be a runner currently
            _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[0],
                startPositions[conn.connectionId].position,
                Quaternion.identity);

            return _temp;
            
        }
        else if (conn.connectionId == 2)
        {
            //Player 3: They are coded to be a runner currently
            _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[0],
                startPositions[conn.connectionId].position,
                Quaternion.identity);

            return _temp;

        }
        else if (conn.connectionId == 3)
        {
            //Player 4: They are coded to be an overseer currently
            _temp = (GameObject)GameObject.Instantiate(spawnPrefabs[1],
                startPositions[conn.connectionId].position,
                Quaternion.identity);

            return _temp;

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
