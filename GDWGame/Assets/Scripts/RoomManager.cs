using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum RoomTraps
{
    NO_TRAP = 0,
    SHOCK = 1,
    EMP = 2,
    SAFEGUARD = 2
}

public enum TrapTypes
{
    WIRED = 0,
    REINFORCED = 1,
    SMOKE = 2,
    NO_TRAP = 3
}

public enum ColourTypes
{
    RED = 0, //0 is false
    GREEN = 1,
    YELLOW = 2
}


//used atiya and angela's flyweight pattern as a framework

public class RoomManager : NetworkBehaviour
{
    public string strZap = "Zap";
    RoomScript[] theRooms;
    Objective[] theObjectives;

    Mesh[] theMeshes = new Mesh[3];
    Material[] theMaterials = new Material[3];
    TrapBase[] theTraps = new TrapBase[3];

    public GameObject emptyObjectives;
    public GameObject emptyRooms;
    List<GameObject> securityBoxes = new List<GameObject>();

    //public MeshFilter wiredMesh, reinforcedMesh, smokeMesh;
    // public Material wiredMat, reinforcedMat, smokeMat;

    //here we can do stuff like the player transporting to a random level, and anything else that would require access to the diff levels. 
    // Use this for initialization
    void Start()
    {
        
        LoadProperties();

        theObjectives = emptyObjectives.GetComponentsInChildren<Objective>();
        theRooms = emptyRooms.GetComponentsInChildren<RoomScript>();

        for (int i = 0; i < theRooms.Length; i++)
        {
            theRooms[i].roomTag = i;
            securityBoxes.Add(theRooms[i].securityBox);
        }

        for (int i = 0; i < theObjectives.Length; i++)
        {
            if (theObjectives[i].objTrapType != TrapTypes.NO_TRAP)
            {
                theObjectives[i].SetUpTrap(theTraps[(int)theObjectives[i].objTrapType]);
            }
        }

        
    }

    void LoadProperties()
    {
        GameObject wiredMesh = Resources.Load("Wired") as GameObject;
        theMeshes[0] = wiredMesh.GetComponent<MeshFilter>().sharedMesh; //wiredMesh.sharedMesh; //

        GameObject reinforcedMesh = Resources.Load("Reinforced") as GameObject;
        theMeshes[1] = reinforcedMesh.GetComponent<MeshFilter>().sharedMesh; //reinforcedMesh.sharedMesh;//

        GameObject smokeMesh = Resources.Load("Smoke") as GameObject;
        theMeshes[2] = smokeMesh.GetComponent<MeshFilter>().sharedMesh; //smokeMesh.sharedMesh; //

        theMaterials[0] = Resources.Load("WiredMat") as Material;
        theMaterials[1] = Resources.Load("ReinforcedMat") as Material;
        theMaterials[2] = Resources.Load("SmokeMat") as Material;

        theTraps[0] = new WiredCase();
        theTraps[0].theMesh = theMeshes[0];
        theTraps[0].theMaterial = theMaterials[0];

        theTraps[1] = new ReinforcedCase();
        theTraps[1].theMesh = theMeshes[1];
        theTraps[1].theMaterial = theMaterials[1];

        theTraps[2] = new SmokeBomb();
        theTraps[2].theMesh = theMeshes[2];
        theTraps[2].theMaterial = theMaterials[2];
    }


    public void Teleport(ref Vector3 position, ref int roomInt)
    {
        int temp = Random.Range(0, theRooms.Length);
        position = theRooms[temp].transform.position;
        roomInt = temp;
        print("Player teleported to room #" + roomInt);

    }

    public Objective Scramble(Objective currentObjective)
    {
        for (int i = 0; i < theObjectives.Length; i++)
        {
            if (currentObjective != theObjectives[i] && theObjectives[i].gameObject.activeSelf)
            {
                print("BeepBoop the new code is: " + theObjectives[i].trapCode[0] + " " + theObjectives[i].trapCode[1] + " " + theObjectives[i].trapCode[2] + " " + theObjectives[i].trapCode[3]);
                return theObjectives[i];
            }
        }

        return currentObjective;
    }

    public Vector3 GetObjPos(int index)
    {
        return theObjectives[index].transform.position;
    }

    public SyncListInt GetObjCode(int index)
    {
        return theObjectives[index].trapCode;
    }

    public RoomTraps getRoomTrap(int index)
    {
        return theRooms[index].trapType;
    }

    public Transform GetSecurityPos(int index)
    {
        return securityBoxes[index].transform;
    }

    public Vector3 GetSecurityRotation(int index)
    {
        return securityBoxes[index].transform.eulerAngles;
    }

    public int ObjectiveLength()
    {
        return theObjectives.Length;
    }

    public int SecurityLength()
    {
        return securityBoxes.Count;
    }
}
