using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using SoundEngine;
using XBOX;

public enum CurrentAbility
{
    zapper = 0,
    shield = 1,
    smokebomb = 2
}

public class PlayerLogic : NetworkBehaviour {
    //enum data
    [HideInInspector]
    private enum runnerStates
    {
        neutral = 0, isTrapped = 1, isZapped = 2, isTeleported = 3, isObtainedObjective = 4, isDisarmedTrap = 5
    }

    [SyncVar] CurrentAbility theCurrentAbility = CurrentAbility.zapper;

    //private variables
    //strings
    string doorStr = "Door", zapStr = "Zap", treasureStr = "Treasure", trapStr = "Trap";
    string runnerOneStr = "RunnerOne", runnerTwoStr = "RunnerTwo", networkTrapStr = "TheNetworkTrap",
        O1 = "Objective", O2 = "Objective2", O3 = "Objective3", O4 = "Objective4", O5 = "Objective5", O6 = "Objective5", O7 = "Objective7", O8 = "Objective8", O9 = "Objective9",
        strBulletPool = "BulletPool", strMouseScrollWheel = "Mouse ScrollWheel", strObjectiveLoc = "ObjectiveLocationTrigger";


    //data that is shared
    [HideInInspector] public int roomInt;
    [HideInInspector] public string playerTag;
    [SyncVar]
    [HideInInspector] public Vector3 playerPosition;
    [HideInInspector] public bool shockTrap = false;
    [HideInInspector] [SyncVar] public int numTreasures = 0;

    //data that is obtained
    public Slider zapperSlider;
    public Slider smokeSlider;
    public Slider shieldSlider;
    public Image zapperFill;
    public RawImage tutorial;
    public RawImage timerImage;
    public RawImage theShieldImg; //these 3 are for specific 
    public GameObject[] PlayerUI; //this is how we set which ui is active in the screen. important cuz itll include misc images. in an array for optimization sake
    public GameObject[] PlayerUIWrap;
    public GameObject Zapper;
    public GameObject smokeBomb;
    public Movement thePlayerMovement;
    public GameObject theShield;
    RoomManager theRoomManager;
    public GameObject lightningSprite;
    OverSeerControl theOverSeer;
    [HideInInspector] public Objective R1currentObjective;
    [HideInInspector] public Objective R2currentObjective;
    GameObject networkZapper;
    public Text scoreText;
    public List<GameObject> theHealthUI;
    int activeBulletNum = 14, numBullets = 15;
    bool canUseSmokeBomb = true; //what a name
    bool canUseShield = true; 
    float smokeCooldownTime = 0.0f;
    float shieldCooldownTime = 0.0f;
    float shieldActiveTime = 0.0f;
    float tutorialCounter = 0.0f;
    [SyncVar] bool shieldActive = false;
    List<int> pickedUpObjectives = new List<int>(); //the reason why im using ints instead of gameobjects is cuz
    //each objective has a tag associated with it, instead of potentially wasting memory by keeping a list of ints, i can save that memory by keeping track of the tags for the objective

    public int currstate;
	[SyncVar]
	public int runID;

    public PlayerObjectiveManager theObjManager; //why'd i bother encapsulating the code if it was gonna be made obsolete like this T_T

    public RunnerParticleSystem theParticleSystem;

	
    //ints, floats and bools used in logic
    int zapperAmount = 3,  damageAttempts = 0; //first for reload, secong for shock/zaps
    [SyncVar]
 //   int zapHealth = 3;
    int zapUI = 0;
    //[SyncVar] dont think I would need this...?
    int trapHealth = 3;

    //UI Animators
    Animator spriteAnim;
    Animator AbilityAnim;

    float playerHeightAmount = 1.5f, shockTimer = 0;
    bool zapperReload = false;
    bool shieldReload = false;
    
    public Canvas playerCanvas;

    bool initRM = false;

    //object pooling variables
    List<GameObject> theBullets = new List<GameObject>();
    TheBulletPool bulletPool;

    //animator for shooting mask
    Animator theAnimator;
    float shootTime;
    bool pickupAllowed = true; 
    float pickupTimer = 0; //to make sure that the player doesnt pick up objective while teleporting

    //for the character select
    [SyncVar] int chosenCharacter;
    [HideInInspector] [SyncVar] public int team = 0;


    public GameObject runPostProcess;

    [SyncVar] public bool inTrap = false; //this is bad. I couldnt find if we already defined this somewhere or not so TwT
    public Texture tutorialOff, tutorialOn;

    //this is for the overseer ui stuff
    [HideInInspector] public RunnerLocUI runnerLocationUI;
    [HideInInspector] public GameObject pDUI, eDUI; //to update the two ui positions.... odear

    public Collider[] colliders;
    
    // Use this for initialization
    void Start () {
        zapUI = 0;
        roomInt = 8;
        
        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        playerTag = gameObject.tag;

        theAnimator = GetComponent<Animator>();

       Shield(false);
        theCurrentAbility = CurrentAbility.zapper;
    }

    public void SetChosenCharacter(int num)
    {
        chosenCharacter = num;
    }

    public void SetChosenTeam(int num)
    {
        team = num; //sets the team
        CmdSetTeam(team);

    }

    [Command]
    void CmdSetTeam(int num)
    {
        team = num;
        RpcSetTeam(num);
    }

    [ClientRpc]
    void RpcSetTeam(int num)
    {
        team = num;
    }

    public void SetRunnerTag(string theTag)
    {
        playerTag = theTag;
        gameObject.tag = theTag;
    }

    [Command]
    void CmdSetCharacter()
    {
        //GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = theRoomManager.characterMeshes[chosenCharacter];
        RpcSetCharacter();
    }

    [ClientRpc]
    void RpcSetCharacter()
    {
        //GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = theRoomManager.characterMeshes[chosenCharacter];
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
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial.SetColor("_RimLight", Color.grey);
            theAnimator = GetComponent<Animator>();
            CmdSetCharacter();
            AbilityAnim = playerCanvas.GetComponent<Animator>();
            spriteAnim = playerCanvas.GetComponentInChildren<Animator>();
            theParticleSystem.TeleportIn();
            initRM = true;
            runPostProcess = GameObject.Find("RunnerPostVolume");
            if (isLocalPlayer)
            {
                GameObject.Find("OverseerPostVolume").SetActive(false);
                runPostProcess.SetActive(true);
            }

            playerPosition = theRoomManager.teleportPositions[roomInt].transform.position;

            //this is to fix the one issue with the health not being right
            if (damageAttempts >= 3)
                damageAttempts = 0;

            theHealthUI[zapUI].SetActive(false);
            zapUI = 0;
            theHealthUI[zapUI].SetActive(true);

        }

        //tutorial thing
        if (tutorialCounter < 8)
        {
            tutorialCounter += Time.deltaTime;
            if (tutorialCounter >= 8)
            {
                timerImage.texture = tutorialOff;
                tutorial.gameObject.SetActive(false);
            }
        }
       
        UpdateOverseerUI();

        if (theRoomManager == null)
            theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();


        KeyInputUpdate();

        ControllerInputUpdate();

        ZapperUpdate();

        TrapAffectUpdate();

        shootAnimUpdate();

        if (!canUseSmokeBomb)
            SmokeCooldown();

        if (!canUseShield)
            ShieldCooldown();

        if (shieldActive == true)
        {
            shieldActiveTime += Time.deltaTime;
            if (shieldActiveTime >= 10)
            {
                shieldActive = false;
                Shield(false);
                shieldActiveTime = 0.0f;
            }
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
            case 8:
                R1currentObjective = GameObject.Find(O8).GetComponentInChildren<Objective>();

                break;
            case 9:
                R1currentObjective = GameObject.Find(O9).GetComponentInChildren<Objective>();

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
            case 8:
                R2currentObjective = GameObject.Find(O8).GetComponentInChildren<Objective>();

                break;
            case 9:
                R2currentObjective = GameObject.Find(O9).GetComponentInChildren<Objective>();

                break;
            default:
                //swing and a miss
                break;
        }
	}
    
    void UpdateOverseerUI()
    {
        if (hasAuthority)
        {
            if (isServer)
                RpcUpdateOverseerUI();
            else
                CmdUpdateOverseerUI();
        }
       
    }

    [Command]
    void CmdUpdateOverseerUI()
    {
        RpcUpdateOverseerUI();
    }

    [ClientRpc]
    void RpcUpdateOverseerUI()
    {
        //to update overseer uis
        if (pDUI != null)
        {
            pDUI.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
        }
        if (eDUI != null)
        {
            eDUI.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
        }

    }

    //updating the key inputs
    private void KeyInputUpdate()
    {
        if (Input.GetMouseButtonDown(0) && inTrap == false)
        {
            RightClickEvents();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            theParticleSystem.IsShot();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tutorial.gameObject.SetActive(!tutorial.gameObject.activeSelf);
            switch(tutorial.gameObject.activeSelf)
            {
                case true:
                    timerImage.texture = tutorialOn;
                    break;
                case false:
                    timerImage.texture = tutorialOff;
                    break;
            }
        }

        if (Input.GetAxis(strMouseScrollWheel) > 0)
        {
            //AbilityAnim.SetBool("SelSmokeUp", true);
            if ((int)theCurrentAbility > 0 && (int)theCurrentAbility <= 2)
            {
                theCurrentAbility--;
                //print(theCurrentAbility);
                ActivateSpecificUI((int)theCurrentAbility);
            }
            //Shield(false);
            //AbilityAnim.SetFloat("Speed Multiplier", -1.0f);
        }

        if (Input.GetAxis(strMouseScrollWheel) < 0)
        {
            AbilityAnim.SetBool("SelShieldDown", true);
            if ((int)theCurrentAbility < 2 && (int)theCurrentAbility >= 0)
            {
                theCurrentAbility++;
                //    print(theCurrentAbility);
                ActivateSpecificUI((int)theCurrentAbility);
            }
            //Shield(false);
            //AbilityAnim.SetFloat("Speed Multiplier", 1.0f);

        }
    }

    private bool ControllerInputUpdate()
    {
        if(XBoxInput.GetConnected())
        {
            if(XBoxInput.GetKeyPressed(0, (int)Buttons.RTrig) && inTrap == false)
            {
                RightClickEvents();
            }

            if (XBoxInput.GetKeyPressed(0, (int)Buttons.LB))
            {
                if ((int)theCurrentAbility > 0)
                    theCurrentAbility--;
                //print(theCurrentAbility);
                ActivateSpecificUI((int)theCurrentAbility);
                //Shield(false);
            }

            if (XBoxInput.GetKeyPressed(0, (int)Buttons.RB))
            {
                if ((int)theCurrentAbility < 2)
                    theCurrentAbility++;
                //    print(theCurrentAbility);
                ActivateSpecificUI((int)theCurrentAbility);
               // Shield(false);
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    //These are all the possible events that can happen with the right mouse click
    private void RightClickEvents()
    {
        if (theCurrentAbility == CurrentAbility.zapper && zapperReload == false && shockTrap == false)
        {
            Shoot(); //shoots the bullet
        }
        else if (theCurrentAbility == CurrentAbility.shield)
        {
            if (canUseShield) //activates shield if it's possible
            {
                Shield(true);
                canUseShield = false;
                shieldSlider.value = 0;
            }
        }
        else if (theCurrentAbility == CurrentAbility.smokebomb) //uses the smokebomb if it's set to it
        {
            if (canUseSmokeBomb) //if the smokebomb is usable
            {
                Smoke();
                canUseSmokeBomb = false;
                smokeSlider.value = 0;
            }
        }

        if (zapperSlider.value <= 0) //this reloads the zapper if it's below a certian point
        {
            zapperReload = true;
            zapperFill.color = Color.red;
        }

        if (activeBulletNum < 0) 
        {
            activeBulletNum = numBullets - 1;
        }
    }

    //shoots the zapper
    private void Shoot()
    {
        if (zapperSlider.value > 0 && activeBulletNum >= 0)
        {
            theAnimator.SetBool("IdleShoot", true);
            shootTime = 0;
            theAnimator.SetTrigger("Shooting");
            zapperSlider.value--;
            CmdSpawnBullet(thePlayerMovement.transform.position, thePlayerMovement.playerCam.rotation, runID);
        }
    }

    //use the smoke bomb
    private void Smoke()
    {
        //smokeBomb.transform.position = thePlayerMovement.transform.position;
        //GameObject bomb = Instantiate(smokeBomb, transform.position + transform.forward * 0.25f, transform.rotation);
        //GameObject bomb = Instantiate(smokeBomb, new Vector3(transform.position.x, 1, transform.position.z), transform.rotation);
        //bomb.GetComponent<SmokeBomb>().SetActive(true);
        CmdSpawnSmoke(thePlayerMovement.transform.position, thePlayerMovement.playerCam.rotation);
    }

    //function to control the cooldown of the smoke bomb
    private void SmokeCooldown()
    {
        smokeCooldownTime += Time.deltaTime;
        smokeSlider.value += Time.deltaTime;
        if (smokeCooldownTime >= 60)
        {
            canUseSmokeBomb = true;
            smokeCooldownTime = 0.0f;
        }
    }

    //function to control the cooldown of the shield bomb
    private void ShieldCooldown()
    {
        shieldCooldownTime += Time.deltaTime;
        shieldSlider.value += Time.deltaTime;
        
        if (shieldCooldownTime >= 30)
        {
            canUseShield = true;
            shieldCooldownTime = 0.0f;
        }

       
    }

    //activates the shield
    public void Shield(bool active)
    {
        if(hasAuthority)
        CmdActivateShield(active);
    }

    //for the shield
    [ClientRpc]
    void RpcActivateShield(bool active)
    {
        theShield.SetActive(active);
        shieldActive = active;
    }

    [Command]
    void CmdActivateShield(bool active)
    {
        RpcActivateShield(active);
        theShield.SetActive(active);

    }

    public void ActivateSpecificUI(int theAbility)
    {
        if (theAbility == 0)
        {
            SetUIAnimation(false, true, false);
        }
        else if (theAbility == 1)
        {
            SetUIAnimation(false, false, true);
        }
        else if (theAbility == 2)
        {
            SetUIAnimation(true, false, false);

        }
    }

    void SetUIAnimation(bool smoke, bool zapper, bool shield)
    {
        AbilityAnim.SetBool("SelZapper", zapper);
        AbilityAnim.SetBool("SelShield", shield);
        AbilityAnim.SetBool("SelSmoke", smoke);
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
                zapperFill.color = Color.blue;
                zapperReload = false;
            }
        }
    }

    //the shield, this is zapper copy paste
    private void ShieldUpdate()
    {
        //copy paasteeeee
        if (shieldReload == true)
        {
            shieldSlider.value += (3 * Time.deltaTime);

            if (shieldSlider.value >= 2 /*&& AreBulletsAvailable()*/)
            {
                shieldSlider.value = 2;
                //zapperFill.color = Color.blue;
                shieldReload = false;
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

        if (!pickupAllowed)
        {
            pickupTimer += Time.deltaTime;
            if (pickupTimer>2)
            {
                pickupTimer = 0;
                pickupAllowed = true;
            }
        }
    }

    private void shootAnimUpdate()
    {
        if (shootTime < 1)
        {
            shootTime += Time.deltaTime;
            if (shootTime > 0.4)
                theAnimator.SetBool("IdleShoot", false);
        }
    }
    
    [ClientRpc]
    void RpcShootBullet(Vector3 position, Quaternion rotation, int IDforBullet)
    {
        //if (isServer)
        //NetworkServer.Spawn(bullet);
        var bullet = GameObject.Find(strBulletPool).GetComponent<TheBulletPool>().availableBullet(position);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y + 2f, bullet.transform.position.z) + transform.forward * 0.2f;
        bullet.transform.rotation = rotation;
        bullet.GetComponent<ZapperScript>().zapperID = IDforBullet;
        
        StartCoroutine(DeactivateBullet(bullet, 3));
    }

    [ClientRpc]
    void RpcUseSmokeBomb(Vector3 position, Quaternion rotation)
    {
        var smokebomb = GameObject.Find(strBulletPool).GetComponent<TheBulletPool>().availableSmokeBomb(position);

        //sounds will go here
    }

    //lovely commands for all the lovely stuff
    [Command]
    void CmdSpawnBullet(Vector3 position, Quaternion rotation, int IDforBullet)
    {
        RpcShootBullet(position, rotation, IDforBullet);
 
    }

    //smoke bomb command
    [Command]
    void CmdSpawnSmoke(Vector3 position, Quaternion rotation)
    {
        RpcUseSmokeBomb(position, rotation);

    }

    //commands work when tied to player objects
    [Command]
    public void CmdDeactivateTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        //  GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
        GameObject.Find(theName).GetComponent<Objective>().RpcDecoupleTrap();
    }

    //I want the sweet relief of death
    [Command]
    public void CmdActivateTrap(string theName)
    {
        //weeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        //  GameObject.Find(theName).GetComponent<Objective>().trapActive = !GameObject.Find(theName).GetComponent<Objective>().trapActive;
        GameObject.Find(theName).GetComponent<Objective>().trapActive = true;
    }

    //I want the sweet relief of death x 2
    [Command]
    public void CmdActivateDoor(string theName, bool temp)
    {
        GameObject.Find(theName).GetComponent<RoomScript>().DoorTrapActive(temp);
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
        damageAttempts++;
        UpdateHealthUI();
        lightningSprite.SetActive(true);

        if (damageAttempts >= 3)
        {
            if (numTreasures > 0)
            {
                //numTreasures--;
                int treasure = numTreasures - 1;
                SetNumTreasure(treasure);
                scoreText.text = numTreasures.ToString();
            }
            Teleport();
            Restore();
        }
    }

    //restores the players stats if the traps affects are done,
    //or if the zapper refills
    public void Restore()
    {
        transform.position = new Vector3(playerPosition.x, playerPosition.y + 1, playerPosition.z);
        
        //updating the damage attempts
        if (damageAttempts >= 3)
            damageAttempts = 0;

        theHealthUI[zapUI].SetActive(false);
        zapUI = 0;
        theHealthUI[zapUI].SetActive(true);
    }

    //activaetes shock if that's what the room trap is
    void SetTrap(RoomTraps theTrapTypes)
    {
        switch (theTrapTypes)
        {
            case RoomTraps.SHOCK:
                ActivateShock();
                break;
        }
    }
    
    
    //sees if the minigame is activated
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
        if (gameObject.name == runnerOneStr && R1currentObjective != null)
            R1currentObjective.minigameActivated = temp;

        if (gameObject.name == runnerTwoStr && R2currentObjective != null)
            R2currentObjective.minigameActivated = temp;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(strObjectiveLoc))
        {
            roomInt = other.GetComponentInParent<ObjectiveLocationTrigger>().locationInt;
            SetRoomInt(roomInt);
        }
        if (other.CompareTag(doorStr))
        {
            if (other.GetComponentInParent<RoomScript>().uponEntering(ref roomInt))
            {
                if (other.GetComponentInParent<RoomScript>().uponEntering(ref roomInt))
                {
                    theAnimator.Play("Shock/Teleport");
                    SetTrap(other.GetComponentInParent<RoomScript>().trapType);
                }
				currstate = (int)runnerStates.isTrapped;
            }
        }
        else if (other.CompareTag(zapStr))
        {
            if (other.GetComponent<ZapperScript>().zapperID != runID /*&& theCurrentAbility != CurrentAbility.shield*/)
            {
                other.gameObject.SetActive(false);
                theParticleSystem.IsShot();

                if (shieldActive != true)
                {
                    damageAttempts++;
                    UpdateHealthUI();
                }
                
                if (damageAttempts >= 3)
                    {
                    pickupAllowed = false;
                    theObjManager.onZapOrQuit();
                    Teleport();
                    if (numTreasures > 0)
                    {
                        Drop();
                        print("it gets to the drop function?");
                    }
                    Restore();

                }


                currstate = (int)runnerStates.isZapped;

				//Debug.Log(zapHealth);
			}
            
        }
        else if (other.CompareTag(treasureStr) && pickupAllowed)
        {
            //CmdScore();
            int number = numTreasures + 1;
            SetNumTreasure(number);
            scoreText.text = numTreasures.ToString();
            other.GetComponent<Treasure>().Deactivate();
            pickedUpObjectives.Add(other.GetComponent<Treasure>().GetTrapID());
			currstate = (int)runnerStates.isObtainedObjective;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(trapStr))
        {
            inTrap = true;
            print("Printing " + inTrap);
            theAnimator.SetBool("Hacking", true);
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(trapStr))
        {
            inTrap = false;
            print("Printing " + inTrap);

            theAnimator.SetBool("Hacking", false);
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

    public void PlayerReshuffle()
    {
        //what am i doingggggg its so late at nighttttttt ahhhh
        if (gameObject.name == runnerOneStr)
        {
            R1currentObjective.Reshuffle(theRoomManager);         
        }
        if (gameObject.name == runnerTwoStr)
        {
            R2currentObjective.Reshuffle(theRoomManager);
        }
        
    }

    public void TrapFailure()
    {
        //if you fail to do the trap
        trapHealth--;
        if (trapHealth <= 0)
        {
            Teleport();
            trapHealth = 3;
        }

        PlayerReshuffle();

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

    void UpdateHealthUI()
    {
       // spriteAnim.SetTrigger("Hit");
        theHealthUI[zapUI].SetActive(false);
        if (zapUI < 2)
            zapUI++;
        theHealthUI[zapUI].SetActive(true);
    }

    void Teleport()
    {
        //print("Before the function call +"+ transform.position);
        //Vector3 dropPosition = new Vector3(transform.position.x, 2, transform.position.z);
        if (hasAuthority)
        {
            if (isServer)
                RpcTeleport(transform.position);
            else
                CmdTeleport(transform.position);
        }
    }

    [Command]
    void CmdTeleport(Vector3 dropPosition)
    {
        RpcTeleport(dropPosition);
    }

    [ClientRpc]
    void RpcTeleport(Vector3 dropPosition)
    {
        playerPosition = transform.position;
        theParticleSystem.TeleportOut();
        //calls the teleport function
        SetRoomInt(roomInt);
        theRoomManager.Teleport(ref playerPosition, ref roomInt, ref pickedUpObjectives, dropPosition);

        //updates minimap ui
    }
    void SetNumTreasure(int num)
    {
        if (hasAuthority)
        {
            if (isServer)
                RpcSetNumTreasure(num);
            else
                CmdSetNumTreasure(num);
        }
    }

    [Command]
    void CmdSetNumTreasure(int num)
    {
        RpcSetNumTreasure(num);
    }

    [ClientRpc]
    void RpcSetNumTreasure(int num)
    {
        numTreasures = num;
        scoreText.text = numTreasures.ToString();
    }

    void Drop()
    {
        if (isServer)
            RpcDrop();
        else
            CmdDrop();
    }

    [Command]
    void CmdDrop()
    {
        RpcDrop();
    }

    [ClientRpc]
    void RpcDrop()
    {
        //sets the treasure stuff
        int treasure = numTreasures - 1;
        SetNumTreasure(treasure);
        scoreText.text = numTreasures.ToString();
    }

    void SetRoomInt(int index)
    {
        if (isServer)
            RpcSetRoomInt(index);
        else
            CmdSetRoomInt(index);
    }

    [Command]
    void CmdSetRoomInt(int index)
    {
        RpcSetRoomInt(index);
    }

    [ClientRpc]
    void RpcSetRoomInt(int index)
    {
        //sets the treasure stuff
        if (runnerLocationUI != null) runnerLocationUI.SetPosition(roomInt);
        roomInt = index;
        if (runnerLocationUI != null) runnerLocationUI.SetPosition(roomInt);

    }

    //setting the room pos
    void SetRoomUI(int index)
    {
        if (isServer)
            RpcSetRoomUI(index);
        else
            CmdSetRoomUI(index);
    }

    [Command]
    void CmdSetRoomUI(int index)
    {
        RpcSetRoomInt(index);
    }

    [ClientRpc]
    void RpcSetRoomUI(int index)
    {
        //sets the new room pos
    }

}
