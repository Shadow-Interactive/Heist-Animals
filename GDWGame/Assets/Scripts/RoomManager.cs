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
    public RoomScript[] theRooms;
    public GameObject[] teleportPositions;
    public Objective[] theObjectives;

    Mesh[] theMeshes = new Mesh[3];
    Material[] theMaterials = new Material[3];
    TrapBase[] theTraps = new TrapBase[3];

    //the meshes and stuff
    public Mesh[] characterMeshes = new Mesh[4];
    public Material[] characterMaterials = new Material[4];

    public GameObject emptyObjectives;
    public GameObject emptyRooms;
    public List<GameObject> securityBoxes = new List<GameObject>();

    public Texture[] theImages = new Texture[10];
    float deactivateTimer = 0;
    bool usingTimer = true;

    //public MeshFilter wiredMesh, reinforcedMesh, smokeMesh;
    // public Material wiredMat, reinforcedMat, smokeMat;

    //here we can do stuff like the player transporting to a random level, and anything else that would require access to the diff levels. 
    // Use this for initialization

    [HideInInspector] [SyncVar] public bool activateReshuffle = false;

    [HideInInspector] [SyncVar] public int reshuffleID = 0;

    bool initRm = false;
    [SyncVar] [HideInInspector] public bool updateUIPosition = false; //hacky solution but simplest one i could think of
    void Start()
    {
        //LoadProperties();
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

        theTraps[2] = new RandomizeCase();
        theTraps[2].theMesh = theMeshes[2];
        theTraps[2].theMaterial = theMaterials[2];

        for (int i = 0; i < theRooms.Length; i++)
        {
            theRooms[i].roomTag = i;
            //securityBoxes.Add(theRooms[i].securityBox);
        }

        for (int i = 0; i < theObjectives.Length; i++)
        {
            if (theObjectives[i].objTrapType != TrapTypes.NO_TRAP)
            {
                theObjectives[i].trapID = i;
                //theObjectives[i].SetUpTrap(theTraps[(int)theObjectives[i].objTrapType]);
            }
        }
    }

    public void Teleport(ref Vector3 position, ref int roomInt, ref List<int> pickedUpObjectives, Vector3 dropPosition)
    {
        int temp = Random.Range(0, teleportPositions.Length);

        //Vector3 dropPosition = new Vector3(position.x, 2.5f, position.z);
        //Vector3 dropPosition = new Vector3(position.x,2,position.z);
        //print("drop position at first" + dropPosition);

        position = teleportPositions[temp].transform.position;
        roomInt = temp;

        if (pickedUpObjectives.Count > 0)
        {
            updateUIPosition = true;
            ObjectiveDrop(pickedUpObjectives[pickedUpObjectives.Count - 1], dropPosition);
            pickedUpObjectives.RemoveAt(pickedUpObjectives.Count - 1);
        }

        //print("drop position after" + dropPosition);

    }

    public void theDrop(ref List<int> pickedUpObjectives, Vector3 dropPosition)
    {
        if (pickedUpObjectives.Count > 0)            
        { 
            updateUIPosition = true;
            ObjectiveDrop(pickedUpObjectives[pickedUpObjectives.Count - 1], dropPosition);
            //print("and at the end..." + dropPosition);
            pickedUpObjectives.RemoveAt(pickedUpObjectives.Count - 1);
        }

    }

    public Objective Scramble(Objective currentObjective)
    {
        for (int i = 0; i < theObjectives.Length; i++)
        {
            if (currentObjective != theObjectives[i] && theObjectives[i].gameObject.activeSelf)
            {
                //print("BeepBoop the new code is: " + theObjectives[i].trapCode[0] + " " + theObjectives[i].trapCode[1] + " " + theObjectives[i].trapCode[2] + " " + theObjectives[i].trapCode[3]);
                return theObjectives[i];
            }
        }

        return currentObjective;
    }

    private void Update()
    {

        if (!initRm)
        {
            LoadProperties();
            initRm = true;
        }

        //THIS IS BECUZ WE COULDN'T FIX THE TRAP ACTIVATION IN OTHER WAYS TOT
        //WILL HOPEFULLY GET CLEANED UP SOON
        //ASK ATIYA FOR DETAILS
        if (usingTimer == true)
        {
            deactivateTimer += Time.deltaTime;

            if (deactivateTimer > 1)
            {
                DeactivateTraps();
                usingTimer = false;
                deactivateTimer = 0;
            }
        }

        if (activateReshuffle == true)
        {
            if (isServer)
            {
                theObjectives[reshuffleID].ActualReshuffle();
                theObjectives[reshuffleID].UpdateSprites(theImages);
                activateReshuffle = false;
            }
        }
        
        for (int i = 0; i < ObjectiveLength(); i++)
        {
            theObjectives[i].UpdateSprites(theImages);
            theObjectives[i].GameObjectVisible(theObjectives[i].trapActive);
        }

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

    public void SetObjectiveTrapImages(int index6, CodeVisual associated, int whichOne)
    {
        if (whichOne == 1)
        theObjectives[index6].associatedCodeObject1 = associated;

        else if (whichOne == 2)
            theObjectives[index6].associatedCodeObject2 = associated;

    }

    public Texture GetTexture(int index7)
    {
        return theImages[index7];
    }

    public void DeactivateTraps()
    {
        for (int i = 0; i < theObjectives.Length; i++)
        {
            theObjectives[i].trapActive = false;
            theObjectives[i].GameObjectVisible(false);
            
        }
    }

    private void OnApplicationQuit()
    {
        DeactivateTraps();
    }

    public void ObjectiveDrop(int index, Vector3 position)
    {//makes treasure visible
        //changes position
        //theObjectives[index].CmdDrop(position);
        print(position);
        theObjectives[index].Drop(position);
    }
}
