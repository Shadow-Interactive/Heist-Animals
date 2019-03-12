using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class RoomScript : NetworkBehaviour
{
    public int roomTag;
    public RoomTraps trapType;
    [SyncVar]
    public bool trapActivated;
    float doorTimer = 0;
    [SyncVar]
    [HideInInspector] public bool doorCooldown = false;
    public GameObject securityBox;
    Color[] theColors = new Color[3];
    string strZapper = "Zap";
    public DoorTrap[] theDoors;

    PlayerLogic runner1, runner2; //players for syncing objectives
    string runnerStr = "RunnerOne", runner2Str = "RunnerTwo";

    // Use this for initialization
    void Start()
    {
        theColors[0] = Color.red;
        theColors[1] = Color.green;
        theColors[2] = Color.yellow;
        ChangeSecurityColor(Convert.ToInt32(trapActivated));

      //  runner1 = GameObject.Find("RunnerOne").GetComponent<PlayerLogic>();
     //   runner2 = GameObject.Find("RunnerTwo").GetComponent<PlayerLogic>();
    }

    public void Update()
    {
        for (int i = 0; i < theDoors.Length; i++)
        {
            DoorTrapActive(trapActivated);
        }

       //Debug.Log(trapActivated);
        if (doorCooldown == true)
        {
            doorTimer += Time.deltaTime;

            if (doorTimer > 3)
            {
                doorCooldown = false;
                doorTimer = 0;
                CmdChangeColor(Convert.ToInt32(trapActivated));

                //ChangeSecurityColor(Convert.ToInt32(trapActivated));
            }
        }
    }

    public bool uponEntering(ref int playerInt)
    {
        playerInt = roomTag;
       // print("Player entered room #" + roomTag);
        //we can activate whatever traps or whatever else here

        return trapActivated;

    }
    

        [Command]
    public void CmdTrapActivation()
    {
        bool wut = !trapActivated;
        
            CmdTrapActivate(wut);
            RpcTrapActivate(wut);
        
     //   if (runner1 != null) runner1.CmdActivateDoor(gameObject.name, wut);
      //  if (runner2 != null) runner2.CmdActivateDoor(gameObject.name, wut);

        doorCooldown = true;
        // ChangeSecurityColor(2);
        CmdChangeColor(2);
    }

    [Command]
    void CmdTrapActivate(bool takeIn)
    {
        trapActivated = takeIn;
        DoorTrapActive(takeIn);
    }

    [ClientRpc]
    void RpcTrapActivate(bool takeIn)
    {
        trapActivated = takeIn;
        DoorTrapActive(takeIn);
    }

    [Command]
    public void CmdChangeColor(int color)
    {
       // securityBox = GetComponent<NetworkIdentity>();        // get the object's network ID
        //securityBox.GetComponent<Renderer>().material.color = theColors[color];
        RpcChangeColor(color);
    }

    [ClientRpc]
    void RpcChangeColor(int color)
    {
        securityBox.GetComponent<Renderer>().material.color = theColors[color];
    }

    public void ChangeSecurityColor(int color)
    {
        securityBox.GetComponent<Renderer>().material.color = theColors[color];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(strZapper))
        {
            other.GetComponent<ZapperScript>().SetActive(false);

            other.GetComponentInChildren<AudioSource>().Play();
        }
    }

    public void DoorTrapActive(bool temp)
    {
        securityBox.GetComponent<Renderer>().material.color = theColors[Convert.ToInt32(temp)];
        for (int i = 0; i < theDoors.Length; i++)
        {
            theDoors[i].SetActive(temp);
        }
    }
}
