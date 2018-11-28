using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using SoundEngine;

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
    [HideInInspector] public Objective R1currentObjective;
    [HideInInspector] public Objective R2currentObjective;
    GameObject networkZapper;
    public Text scoreText;
    int activeBulletNum = 14, numBullets = 15; 


    //private variables
    //strings
    string doorStr = "Door", zapStr = "Zap", treasureStr = "Treasure", trapStr = "Trap";
    string runnerOneStr = "RunnerOne", runnerTwoStr = "RunnerTwo", networkTrapStr = "TheNetworkTrap", 
        O1 = "Objective", O2 = "Objective2", O3 = "Objective3", O4 = "Objective4", O5 = "Objective5", O6 = "Objective5", O7 = "Objective7",
        strBulletPool = "BulletPool";

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
    TheBulletPool bulletPool;

    // Use this for initialization
    void Start () {
        roomInt = 8;
        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        playerTag = gameObject.tag;


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

        //for deactivating runner 1 traps :)
        switch (GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().trapToGoAway)
        {
            case 1:
                R1currentObjective = GameObject.Find(O1).GetComponentInChildren<Objective>();

                break;
            case 2:
                R1currentObjective = GameObject.Find(O2).GetComponentInChildren<Objective>();

                break;
            case 3:
                R1currentObjective = GameObject.Find(O3).GetComponentInChildren<Objective>();

                break;
            case 4:
                R1currentObjective = GameObject.Find(O4).GetComponentInChildren<Objective>();

                break;
            case 5:
                R1currentObjective = GameObject.Find(O5).GetComponentInChildren<Objective>();

                break;
            case 6:
                R1currentObjective = GameObject.Find(O6).GetComponentInChildren<Objective>();

                break;
            case 7:
                R1currentObjective = GameObject.Find(O7).GetComponentInChildren<Objective>();

                break;
            default:
                //swing and a miss
                break;
        }

        //for deactivating runner2 traps :)
        switch (GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().otherTrapToGoAway)
        {
            case 1:
                R2currentObjective = GameObject.Find(O1).GetComponentInChildren<Objective>();

                break;
            case 2:
                R2currentObjective = GameObject.Find(O2).GetComponentInChildren<Objective>();

                break;
            case 3:
                R2currentObjective = GameObject.Find(O3).GetComponentInChildren<Objective>();

                break;
            case 4:
                R2currentObjective = GameObject.Find(O4).GetComponentInChildren<Objective>();

                break;
            case 5:
                R2currentObjective = GameObject.Find(O5).GetComponentInChildren<Objective>();

                break;
            case 6:
                R2currentObjective = GameObject.Find(O6).GetComponentInChildren<Objective>();

                break;
            case 7:
                R2currentObjective = GameObject.Find(O7).GetComponentInChildren<Objective>();

                break;
            default:
                //swing and a miss
                break;
        }
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

            if (zapperSlider.value >= zapperAmount /*&& AreBulletsAvailable()*/)
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
        //theBullets[activeBulletNum].GetComponent<ZapperScript>().SetPosition(transform.position + transform.forward * 0.25f, thePlayerMovement.playerCam.rotation);
        //theBullets[activeBulletNum].GetComponent<ZapperScript>().SetActive(true);
        //activeBulletNum--;
        CmdSpawnBullet(thePlayerMovement.transform.position, thePlayerMovement.playerCam.rotation, gameObject.tag);
        //print("Pew" + activeBulletNum);
        //networkZapper = Instantiate(Zapper, transform.position, thePlayerMovement.playerCam.rotation);

        //Debug.Log(Bullet.GetComponent<ZapperScript>().zapperTag);
    }
    [ClientRpc]
    void RpcShootBullet(Vector3 position, Quaternion rotation, string tagForBullet)
    {
        //if (isServer)
        //NetworkServer.Spawn(bullet);
        var bullet = GameObject.Find(strBulletPool).GetComponent<TheBulletPool>().availableBullet(position);
        bullet.transform.position = (bullet.transform.position + transform.forward * 0.25f);
        bullet.transform.rotation = rotation;
        bullet.GetComponent<ZapperScript>().zapperTag = tagForBullet;

        SoundManager.setPlaying(true, 1);
        SoundManager.setLoop(1, false);

        Vector3 vel = gameObject.transform.forward * 20f;
        SoundManager.setVelocity(vel.x, vel.y, vel.y, 1);

        Vector3 pos = GetComponent<Transform>().position;
        SoundManager.setPosition(pos.x, pos.y, pos.z, 1);

        SoundManager.setVolume(20.0f, 1);

        SoundManager.playSound(1, Time.deltaTime);

        StartCoroutine(DeactivateBullet(bullet, 3));
    }

    //lovely commands for all the lovely stuff
    [Command]
    void CmdSpawnBullet(Vector3 position, Quaternion rotation, string tagForBullet)
    {
        //GameObject Bullet = Instantiate(Zapper, transform.position + transform.forward * 0.25f, rotation);
        RpcShootBullet(position, rotation, tagForBullet);
        //NetworkServer.Spawn(bullet);
 
    }

    //commands work when tied to player objects
    [Command]
    public void CmdDeactivateTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
    }

    public IEnumerator DeactivateBullet(GameObject bullet, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Find("BulletPool").GetComponent<TheBulletPool>().returnToPool(bullet);
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
       //if (gameObject.name == runnerOneStr)
       //R1currentObjective = theRoomManager.Scramble(R1currentObjective);
       //
       //if (gameObject.name == runnerTwoStr)
       //R2currentObjective = theRoomManager.Scramble(R2currentObjective);
       
    }

    public bool GetActivation()
    {
        if (gameObject.name ==runnerOneStr)
            return R1currentObjective.minigameActivated;
        else if (gameObject.name == runnerTwoStr)
            return R2currentObjective.minigameActivated;
        else
            return false;
    }

    public void SetActivation(bool temp)
    {
        if (gameObject.name == runnerOneStr)
            R1currentObjective.minigameActivated = temp;

        if (gameObject.name == runnerTwoStr)
            R2currentObjective.minigameActivated = temp;
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

                //Destroy(other.gameObject);
                other.gameObject.SetActive(false);

                //if (!isServer)
                //{
                //    return;
                //}

                zapHealth--;

                SoundManager.setPlaying(true, 2);
                SoundManager.setLoop(2, false);
                
                SoundManager.setVelocity(0f, 0f, 0f, 2);

                Vector3 pos = GetComponent<Transform>().position;
                SoundManager.setPosition(pos.x, pos.y, pos.z, 2);

                SoundManager.setVolume(20.0f, 2);

                SoundManager.playSound(2, Time.deltaTime);

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
            if (gameObject.name == runnerOneStr)
            {
                GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().trapToGoAway = collision.gameObject.GetComponent<Objective>().trapNum;
                R1currentObjective = collision.gameObject.GetComponent<Objective>();
            }

            if (gameObject.name == runnerTwoStr)
            {
                GameObject.Find(networkTrapStr).GetComponent<TrapForNetwork>().otherTrapToGoAway = collision.gameObject.GetComponent<Objective>().trapNum;
                R2currentObjective = collision.gameObject.GetComponent<Objective>();
            }
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
        if (gameObject.name == runnerOneStr)
        {
            R1currentObjective.Reshuffle(theRoomManager);
          //  R1currentObjective.UpdateSprites(theRoomManager.theImages);
            
        }
        if (gameObject.name == runnerTwoStr)
        {
            R2currentObjective.Reshuffle(theRoomManager);
            //R2currentObjective.UpdateSprites(theRoomManager.theImages);

        }
        
    }

    public void TrapFailure()
    {
        trapHealth--;
        if (trapHealth <= 0)
        {
            theRoomManager.Teleport(ref playerPosition, ref roomInt);
            trapHealth = 3;
        }

        Reshuffle();
    }

    public bool AreBulletsAvailable()
    {
        int bulletCounter = 0;
        for (int i = 0; i < theBullets.Count - 1; i++)
        {
            if (!theBullets[i].GetComponent<ZapperScript>().GetActive())
                bulletCounter++;
        }

        return bulletCounter >= 3 ? true : false;
    }
}
