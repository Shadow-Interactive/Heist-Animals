using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DoorTrap : NetworkBehaviour
{
    [SyncVar] [HideInInspector] public bool trapActive = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetActive(bool temp)
    { 
        CmdSetTrapActive(temp);
        gameObject.SetActive(temp);
    }

    [Command]
    public void CmdSetTrapActive(bool temp)
    {
        trapActive = temp;
        RpcSetTrapActive(temp);
    }

    [ClientRpc]
    public void RpcSetTrapActive(bool temp)
    {
        trapActive = temp;
        gameObject.SetActive(trapActive);
    }

}
