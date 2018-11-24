using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public class PlayerLogic : NetworkBehaviour {

    //data that is shared
    [HideInInspector] public int roomInt;
    [HideInInspector] public string playerTag;
    [SyncVar]
    [HideInInspector] public Vector3 playerPosition;
    [HideInInspector] public bool shockTrap = false;
    [HideInInspector] public int numTreasures = 0;

    //data that is obtained
    public Slider zapperSlider;
    public Image zapperFill;
    public GameObject Zapper;
    public Movement thePlayerMovement;
    RoomManager theRoomManager;
    public GameObject lightningSprite;
    OverSeerControl theOverSeer;
    [HideInInspector] public Objective currentObjective;
    GameObject networkZapper;
    public Text scoreText;
    int activeBulletNum = 14, numBullets = 15; 

    [SyncVar]
    public bool trapActive = false;
    [SyncVar(hook = "NumChange")]
    public int curTrap;
    private int test;


    //private variables
    //strings
    string doorStr = "Door", zapStr = "Zap", treasureStr = "Treasure", trapStr = "Trap";

    //ints, floats and bools used in logic
    int zapperAmount = 3,  shockAttempts = 0;
    [SyncVar]
    int zapHealth = 3;
    //[SyncVar] dont think I would need this...?
    int trapHealth = 3;

    float playerHeightAmount = 1.5f, shockTimer = 0;
    bool zapperReload = false;
    
    public Canvas playerCanvas;

    bool initRM = false;

    List<GameObject> theBullets = new List<GameObject>();

    // Use this for initialization
    void Start () {
        roomInt = 8;
        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        playerTag = gameObject.tag;


    }

    public void NumChange(int newValue)
    {
        Debug.Log("Old value is " + curTrap);
        curTrap = newValue;
        Debug.Log("Value is now: " + curTrap);
    }

    [ClientRpc]
    public void RpcUpdateData()
    {
        curTrap = 1;
    }


    public void SetRunnerTag(string theTag)
    {
        playerTag = theTag;
        gameObject.tag = theTag;
    }

    // Update is called once per frame
    void Update () {
        
        if (!isLocalPlayer)
        {
            shockTrap = false;
            playerTag = null;
            playerCanvas.gameObject.SetActive(false);
            return;
        }

        if (!initRM)
        {
            theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();

            for (int i = 0; i < numBullets; i++)
                CmdSpawnBullet(thePlayerMovement.playerCam.rotation, gameObject.tag);

            initRM = true;
        }

        //the updates that are running
        //I think I may switch some of these out for coroutines for the sake of performance later on
        KeyInputUpdate();

        ZapperUpdate();

        TrapAffectUpdate();

        if (zapHealth <= 0)
        {
            theRoomManager.Teleport(ref playerPosition, ref roomInt);
            Restore();
        }

        //Debug.Log(currentObjective.trapActive);
        //Debug.Log(GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway);

        //if (trapActive)
        //curTrap = 1;

        switch(GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway)
        {
            case 1:
                currentObjective = GameObject.Find("Objective").GetComponentInChildren<Objective>();
                break;
            case 2:
                currentObjective = GameObject.Find("Objective2").GetComponentInChildren<Objective>();
                break;
            case 3:
                currentObjective = GameObject.Find("Objective3").GetComponentInChildren<Objective>();
                break;
            case 4:
                currentObjective = GameObject.Find("Objective4").GetComponentInChildren<Objective>();
                break;
            case 5:
                currentObjective = GameObject.Find("Objective5").GetComponentInChildren<Objective>();
                break;
            case 6:
                currentObjective = GameObject.Find("Objective6").GetComponentInChildren<Objective>();
                break;
            case 7:
                currentObjective = GameObject.Find("Objective7").GetComponentInChildren<Objective>();
                break;
            default:
                break;
       

        }

        //currentObjective.DeActivate();

        //if (GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway == 1)
        //{
        //    currentObjective = GameObject.Find("Objective").GetComponentInChildren<Objective>();
        //    Debug.Log(currentObjective.name);
        //    //Debug.Log(currentObjective.trapActive);
        //    //currentObjective.DeActivate();
        //    currentObjective.DeActivate();
        //}
    }

    //updating the key inputs
    private void KeyInputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && zapperReload == false && shockTrap == false)
        {
            if (zapperSlider.value > 0 && activeBulletNum >= 0)
            {
                Shoot();
            }

            if (zapperSlider.value <= 0)
            {
                zapperReload = true;
                zapperFill.color = Color.red;
            }

            if (activeBulletNum < 0)
            {
                activeBulletNum = numBullets - 1 ;
            }
        }
    }

    //updating anything involving the zapper
    private void ZapperUpdate()
    {
        //when the zaps run out, it takes time to refill
        if (zapperReload == true)
        {
            zapperSlider.value += (3 * Time.deltaTime);

            if (zapperSlider.value >= zapperAmount && AreBulletsAvailable())
            {
                zapperSlider.value = zapperAmount;
                zapperFill.color = Color.green;
                zapperReload = false;
            }
        }
    }

    //it checks if any traps are affecting the player, then updates the affect
    private void TrapAffectUpdate()
    {
        if (shockTrap == true)
        {
            shockTimer += Time.deltaTime;
            if (shockTimer >= 3)
            {
                shockTrap = false;
                lightningSprite.SetActive(false);
                thePlayerMovement.disableMovement = shockTrap;
                shockTimer = 0;
            }
        }
    }

    //shoots the zapper
    private void Shoot()
    {
       
       zapperSlider.value--;
       theBullets[activeBulletNum].GetComponent<ZapperScript>().SetPosition(transform.position + transform.forward * 0.25f, thePlayerMovement.playerCam.rotation);
       theBullets[activeBulletNum].GetComponent<ZapperScript>().SetActive(true);
       activeBulletNum--;
        print("Pew" + activeBulletNum);
        //networkZapper = Instantiate(Zapper, transform.position, thePlayerMovement.playerCam.rotation);

        //Debug.Log(Bullet.GetComponent<ZapperScript>().zapperTag);
    }

    //lovely commands for all the lovely stuff
    [Command]
    void CmdSpawnBullet(Quaternion rotation, string tagForBullet)
    {
        
        GameObject Bullet = Instantiate(Zapper, transform.position + transform.forward * 0.25f, rotation);
        //if (isLocalPlayer)
        Bullet.GetComponent<ZapperScript>().zapperTag = tagForBullet;
        theBullets.Add(Bullet);
        NetworkServer.Spawn(Bullet);
    }

    //commands work when tied to player objects
    [Command]
    public void CmdDeactivateTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
        //currentObjective.trapActive = !currentObjective.trapActive;
        //currentObjective.DeActivate();
    }

    //activates the affect on the player for the shock trap
    public void ActivateShock()
    {
        shockTrap = true;
        thePlayerMovement.disableMovement = shockTrap;
        shockAttempts++;
        lightningSprite.SetActive(true);

        if (shockAttempts >= 3)
        {
            theRoomManager.Teleport(ref playerPosition, ref roomInt);
            Restore();
        }
    }

    //restores the players stats if the traps affects are done,
    //or if the zapper refills
    public void Restore()
    {
        transform.position = new Vector3(playerPosition.x, playerPosition.y + 1, playerPosition.z);
        
        if (zapHealth <= 0)
            zapHealth = 3;
        if (shockAttempts >= 3)
            shockAttempts = 0;
    }

    void SetTrap(RoomTraps theTrapTypes)
    {
        switch (theTrapTypes)
        {
            case RoomTraps.SHOCK:
                ActivateShock();
                break;
            case RoomTraps.EMP:
                //theOverSeer.setEMP(true);
                //print(theOverSeer.getEMP());

                break;
        }
    }

    public void Scramble()
    {
        currentObjective = theRoomManager.Scramble(currentObjective);
    }

    public bool GetActivation()
    {
        return currentObjective.minigameActivated;
    }

    public void SetActivation(bool temp)
    {
        currentObjective.minigameActivated = temp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(doorStr))
        {
            //I'M GONNA CHANGE THIS AND CLEAN IT UP
            //THIS IS CODE I DID LAST MINUTE TO FIT WITH THE NEW CHANGES WITH THE RUNNER
            //JUST GIVE ME TIMEEEE
            if (roomInt != other.GetComponentInParent<RoomScript>().roomTag)
            {
                if (other.GetComponentInParent<RoomScript>().uponEntering(ref roomInt))
                {
                    SetTrap(other.GetComponentInParent<RoomScript>().trapType);
                }
            }

        }
        else if (other.CompareTag(zapStr))
        {
            if (other.GetComponent<ZapperScript>().zapperTag != gameObject.tag)
            {
                
                Destroy(other.gameObject);

                //if (!isServer)
                //{
                //    return;
                //}

                zapHealth--;

                //Debug.Log(zapHealth);
            }
            
        }
        if (other.CompareTag(treasureStr))
        {
            numTreasures += other.GetComponent<Treasure>().ScoreWorth; 
            scoreText.text = numTreasures.ToString();
            other.GetComponent<Treasure>().Deactivate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(trapStr))
        {
            trapActive = true;
            GameObject.Find("TheNetworkTrap").GetComponent<TrapForNetwork>().trapToGoAway = collision.gameObject.GetComponent<Objective>().trapNum;
            curTrap = collision.gameObject.GetComponent<Objective>().trapNum;

            //currentObjective = collision.gameObject.GetComponent<Objective>();
            //print(currentObjective.objTrapType);
        }
    }

    public void SetMovement(bool movement)
    {
        thePlayerMovement.disableMovement = movement;
    }

    public void setCursor(bool activation)
    {
        thePlayerMovement.SetCursor(activation);
    }

    public void Reshuffle()
    {
        //what am i doingggggg its so late at nighttttttt ahhhh
        currentObjective.Reshuffle(theRoomManager.theImages);
    }

    public void TrapFailure()
    {
        trapHealth--;
        if (trapHealth <= 0)
        {
            theRoomManager.Teleport(ref playerPosition, ref roomInt);
            trapHealth = 3;
        }
        currentObjective.Reshuffle(theRoomManager.theImages);
    }

    public bool AreBulletsAvailable()
    {
        int bulletCounter = 0;
        for (int i = 0; i < theBullets.Count-1; i++)
        {
            if (!theBullets[i].GetComponent<ZapperScript>().GetActive())
                bulletCounter++;
        }

        return bulletCounter >= 3 ? true : false;
    }
}
