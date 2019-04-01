using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour {

    VideoPlayer cinematic;
    public GameObject level, UI;
    public AudioSource menuMusic;

    bool firstTimeLoad;
	// Use this for initialization
	void Start () {
        cinematic = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoPlayer>();
        firstTimeLoad = false;
        menuMusic.mute = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (cinematic.isPlaying)
        {
            level.SetActive(false);
            UI.SetActive(false);
            if (menuMusic.isPlaying)
                menuMusic.Stop();
            menuMusic.mute = true;
            firstTimeLoad = true;
        }
        else
        {
            if (firstTimeLoad == true)
            {
                level.SetActive(true);
                UI.SetActive(true);
                menuMusic.mute = false;
                if(!menuMusic.isPlaying)
                    menuMusic.Play();
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (cinematic.isPlaying)
            {
                cinematic.Stop();
                cinematic.frame = 0;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (!cinematic.isPlaying)
            {
                cinematic.Play();
            }
        }


    }
}
