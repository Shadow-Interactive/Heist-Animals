using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class SoundManager : MonoBehaviour {

    const string dllName = "FMOD Controller";

    [DllImport(dllName)]
    public static extern void initFMOD();

    [DllImport(dllName)]
    public static extern void setPosition(float _x, float _y, float _z);

    [DllImport(dllName)]
    public static extern void setVelocity(float _x, float _y, float _z);

    [DllImport(dllName)]
    public static extern void update(float dt, int channel, bool _playing);

    [DllImport(dllName)]
    public static extern void setMode(int channel);

    [DllImport(dllName)]
    public static extern void setVolume(float _volume, int channel);

    [DllImport(dllName)]
    public static extern void createSound(string soundFile);

    [DllImport(dllName)]
    public static extern void cleanFMOD();

    // Use this for initialization
    void Start () {
        initFMOD();
	}
	
	// Update is called once per frame
	void Update () {
        update(Time.deltaTime, 0, true);
	}

    void OnDestroy()
    {
        cleanFMOD();
    }
}