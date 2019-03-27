using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using SoundEngine;

public class ZapperScript : NetworkBehaviour {

    [SyncVar]
    public string zapperTag;
    [SyncVar] public int zapperID;
    bool active;

	// Use this for initialization
	void Start () {
        SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (active)
        {
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 80f;
        }
    }

    public void SetActive(bool temp)
    {
        active = temp;
        gameObject.SetActive(temp);
    }

    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation; 
    }

    public bool GetActive()
    {
        return active; 
    }

    public void Ricochet(Vector3 PointOfContact)
    {
        transform.position = Vector3.Reflect(PointOfContact, -gameObject.transform.forward);
        print("Old rotation " + transform.forward);

        //  CmdRicochet(PointOfContact);
    }

    [Command]
    public void CmdRicochet(Vector3 PointOfContact)
    {
        RpcRicochet(PointOfContact);

    }

    [ClientRpc]
    void RpcRicochet(Vector3 PointOfContact)
    {
        transform.position = Vector3.Reflect(PointOfContact, -gameObject.transform.forward);
        // transform.forward = -transform.forward;
        // print("Old rotation " + transform.forward);
        // transform.Rotate(0, 0, 180);
        // print("new rotation " + transform.forward);


    }
}
