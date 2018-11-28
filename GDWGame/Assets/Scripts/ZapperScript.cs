using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using SoundEngine;

public class ZapperScript : NetworkBehaviour {

    [SyncVar]
    public string zapperTag;
    bool active;

	// Use this for initialization
	void Start () {
        SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (active)
        {
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 20f;

            
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

}
