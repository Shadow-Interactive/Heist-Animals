using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    TrapBase theTrapType;
    Vector3 objectivePosition;
    public TrapTypes objTrapType;
    [HideInInspector] public int[] trapCode = new int[4];
    PlayerObjectiveManager activePlayer;
    [HideInInspector] public bool minigameActivated = false; 

    // Use this for initialization
    void Start () {
		for (int i = 0; i <4; i++)
        {
            trapCode[i] = Random.Range(0, 10);
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
                Activate();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (minigameActivated == true)
            minigameActivated = false;
    }

    public void Reshuffle()
    {
        for (int i = 0; i < 4; i++)
        {
            trapCode[i] = Random.Range(0, 10);
        }
        print("New trap is" + trapCode[0] + trapCode[1]+trapCode[2]+trapCode[3]);

    }

    public void Activate()
    {
        gameObject.SetActive(true);
        minigameActivated = true;
        theTrapType.Execute(activePlayer);
    }

    public void DeActivate()
    {
        gameObject.SetActive(false);
    }
    public void GameObjectVisible(bool temp)
    {
        gameObject.SetActive(temp);
    }

    //https://letsgeekblog.wordpress.com/2017/03/11/drag-object-script-in-unity3d/
    void OnMouseDown()
    {
        if (minigameActivated == false)
            GameObjectVisible(false);
    }
}
