using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomSpawn : NetworkLobbyManager {

    public Movement runner;
    public OverSeerControl overseer;
    public Transform Spawn;
    private Transform Spawn2;

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
    }
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);
        RegisterStartPosition(Spawn);

        GameObject _temp;

        if (!overseer.camChoice)
        {
            if (runner.spawnIn)
            {
                _temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Players/Runner 1"),
                    startPositions[conn.connectionId].position,
                    Quaternion.identity);

                return _temp;
            }
            else
                return null;
            
        }
        else if (overseer.camChoice)
        {
            _temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Players/CameraManager"),
                startPositions[conn.connectionId].position,
                Quaternion.identity);
            return _temp;
        }
        else
            return null;
    }
}
