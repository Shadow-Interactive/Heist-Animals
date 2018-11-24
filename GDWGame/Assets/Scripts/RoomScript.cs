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

    // Use this for initialization
    void Start()
    {
        theColors[0] = Color.red;
        theColors[1] = Color.green;
        theColors[2] = Color.yellow;
        ChangeSecurityColor((ColourTypes)Convert.ToInt32(trapActivated));
    }

    public void Update()
    {
       //Debug.Log(trapActivated);
        if (doorCooldown == true)
        {
            doorTimer += Time.deltaTime;

            if (doorTimer > 3)
            {
                doorCooldown = false;
                doorTimer = 0;
                ChangeSecurityColor((ColourTypes)Convert.ToInt32(trapActivated));
            }
        }
    }

    public bool uponEntering(ref int playerInt)
    {
        playerInt = roomTag;
        print("Player entered room #" + roomTag);
        //we can activate whatever traps or whatever else here

        return trapActivated;

    }

    //public void TrapActivation(bool temp) //0 is false, 1 is true
    //{
    //    trapActivated = temp;
    //    doorCooldown = true;
    //    ChangeSecurityColor(ColourTypes.YELLOW);
    //}

    public void TrapActivation()
    {
        //trapActivated = !trapActivated;

        if (isServer)
        {
            trapActivated = !trapActivated;
        }
        else
        {
            CmdTrapActivate(false);
        }
        doorCooldown = true;
        ChangeSecurityColor(ColourTypes.YELLOW);
    }

    [Command]
    void CmdTrapActivate(bool takeIn)
    {
        trapActivated = takeIn;
    }

    public void ChangeSecurityColor(ColourTypes theColour)
    {
        securityBox.GetComponent<Renderer>().material.color = theColors[(int)theColour];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(strZapper))
            other.GetComponent<ZapperScript>().SetActive(false);
    }
}
