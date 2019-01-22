using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SmokeBomb : NetworkBehaviour {

    ParticleSystem smoke;
    bool active;
    float smokeTime = 0;
	// Use this for initialization
	void Start () {

        smoke = GetComponent<ParticleSystem>();
        SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        if (active)
        {
            smoke.Play();
            smokeTime += Time.deltaTime;
        }

        if (smokeTime >= 17)
        {
            SetActive(false);
            smoke.Stop();
            smokeTime = 0;
        }

        //Debug.Log(smoke.time);
        //smoke.time++; //????
        //particle system time doesn't appear to be incrementing?
        if (smoke.time >= 17)
        {
            SetActive(false);
            smoke.Stop();
        }

	}

    public void SetActive(bool temp)
    {
        active = temp;
        gameObject.SetActive(temp);

    }

    public bool GetActive()
    {
        return active;
    }

    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
