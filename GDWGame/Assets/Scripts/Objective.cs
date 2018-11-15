using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Objective : NetworkBehaviour {

    TrapBase theTrapType;
    Vector3 objectivePosition;
    public TrapTypes objTrapType;
    [HideInInspector] public SyncListInt trapCode = new SyncListInt();
    PlayerObjectiveManager activePlayer;
    [HideInInspector] public bool minigameActivated = false;
    [HideInInspector] public CodeVisual associatedCodeObject;

    // Use this for initialization
    void Start () {
		for (int i = 0; i <4; i++)
        {
            int random = Random.Range(0, 10);
            trapCode.Add(random);
        }

        print("Trap is" + trapCode[0] + trapCode[1] + trapCode[2] +  trapCode[3] );
    }

    // Update is called once per frame
    void Update () {

    }

    public void SetUpTrap(TrapBase theType)
    {
        theTrapType = theType;
        GetComponent<MeshFilter>().sharedMesh = theType.theMesh;
        gameObject.GetComponent<Renderer>().sharedMaterial = theType.theMaterial;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!minigameActivated)
        {
            if (collision.collider.CompareTag("RunnerOne") && objTrapType != TrapTypes.NO_TRAP)
            {
                activePlayer = collision.gameObject.GetComponentInChildren<PlayerObjectiveManager>();
                activePlayer.ActivateMinigame();
                ActivateMinigame();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (minigameActivated == true)
            minigameActivated = false;
    }

    public void Reshuffle(Texture[] theImages)
    {
        for (int i = 0; i < 4; i++)
        {
            trapCode[i] = Random.Range(0, 10);
            associatedCodeObject.SetSprite(i, theImages[trapCode[i]]);
        }
        print("New trap is" + trapCode[0] + trapCode[1]+trapCode[2]+trapCode[3]);

    }

    public void ActivateMinigame()
    {
        minigameActivated = true;
        theTrapType.Execute(activePlayer);
    }

    public void ActivateObject(Texture[] theImages)
    {
        gameObject.SetActive(true);
        Reshuffle(theImages);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }
    public void GameObjectVisible(bool temp)
    {
        gameObject.SetActive(temp);
    }
}
