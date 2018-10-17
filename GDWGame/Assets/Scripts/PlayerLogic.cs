using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour {

    //data that is shared
    [HideInInspector] public int roomInt;
    [HideInInspector] public string playerTag;
    [HideInInspector] public Vector3 playerPosition;
    [HideInInspector] public bool shockTrap = false;

    //data that is obtained
    public Slider zapperSlider;
    public Image zapperFill;
    public GameObject Zapper;
    public Movement thePlayerMovement;
    RoomManager theRoomManager;
    public GameObject lightningSprite;

    //private variables
    //strings
    string doorStr = "Door", zapStr = "Zap";

    //ints, floats and bools used in logic
    int zapperAmount = 3, zapHealth = 3, shockAttempts = 0;
    float playerHeightAmount = 1.5f, shockTimer = 0;
    bool zapperReload = false;

    // Use this for initialization
    void Start () {
        roomInt = 8;
        theRoomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
    }

    public void SetRunnerTag(string theTag)
    {
        playerTag = theTag;
        gameObject.tag = theTag;
    }

    // Update is called once per frame
    void Update () {

        //the updates that are running
        //I think I may switch some of these out for coroutines for the sake of performance later on
        KeyInputUpdate();

        ZapperUpdate();

        TrapAffectUpdate();
    }

    //updating the key inputs
    private void KeyInputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && zapperReload == false && shockTrap == false)
        {
            if (zapperSlider.value > 0)
            {
                Shoot();
            }

            if (zapperSlider.value <= 0)
            {
                zapperReload = true;
                zapperFill.color = Color.red;
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

            if (zapperSlider.value >= zapperAmount)
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
        GameObject newZapper = GameObject.Instantiate(Zapper, transform.position, thePlayerMovement.playerCam.rotation);
        newZapper.GetComponent<ZapperScript>().zapperTag = playerTag;
        Destroy(newZapper, 5);
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
        transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);

        if (zapHealth <= 0)
            zapHealth = 3;
        if (shockAttempts >= 3)
            shockAttempts = 0;
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
                other.GetComponentInParent<RoomScript>().uponEntering(ref roomInt, ref shockTrap);
                if (shockTrap == true && shockTimer == 0)
                {
                    ActivateShock();
                }
            }
          
        }
        else if (other.CompareTag(zapStr))
        {
            if (other.GetComponent<ZapperScript>().zapperTag != playerTag)
            {
                Destroy(other.gameObject);
                zapHealth--;

                if (zapHealth <= 0)
                {
                    theRoomManager.Teleport(ref playerPosition, ref roomInt);
                    Restore();
                }
            }
        }
    }

}
