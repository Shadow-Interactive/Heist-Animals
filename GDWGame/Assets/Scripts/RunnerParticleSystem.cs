using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RunnerParticleSystem : NetworkBehaviour {

    public ParticleSystem teleportIn;
    public ParticleSystem teleportOut;
    public ParticleSystem shot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TeleportOut()
    {
        if (hasAuthority)
        {
            if (isServer)
                RpcTeleportOut();
            else
                CmdTeleportOut();
        }
    }

    [Command]
    void CmdTeleportOut()
    {
        RpcTeleportOut();
    }

    [ClientRpc]
    void RpcTeleportOut()
    {
        teleportOut.Play();
    }

    public void TeleportIn()
    {
        if (isServer)
            RpcTeleportIn();
        else
            CmdTeleportIn();
    }

    [Command]
    void CmdTeleportIn()
    {
        RpcTeleportIn();
    }

    [ClientRpc]
    void RpcTeleportIn()
    {
        teleportIn.Play();
    }

    public void IsShot() //for whenthe player gets shot
    {
        if (hasAuthority)
        {
            if (isServer)
                RpcIsShot();
            else
                CmdIsShot();
        }
    }

    [Command]
    void CmdIsShot()
    {
        RpcIsShot();
    }

    [ClientRpc]
    void RpcIsShot()
    {
        print("the shot");
        shot.Play();
    }
}
