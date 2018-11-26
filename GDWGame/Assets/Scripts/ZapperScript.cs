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

        SoundManager.createSound(SoundManager.soundPath + "ZapperFire.mp3", 1);
        SoundManager.setPlaying(true, 1);

        Vector3 vel = gameObject.transform.forward * 20f;
        SoundManager.setVelocity(vel.x, vel.y, vel.y, 1);

        Vector3 pos = GetComponent<Transform>().position;
        SoundManager.setPosition(pos.x, pos.y, pos.z, 1);

        SoundManager.setVolume(3.0f, 1);

        SoundManager.playSound(1, Time.deltaTime);
    }

    // Update is called once per frame
    void Update () {
        if (active)
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 20f;
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
