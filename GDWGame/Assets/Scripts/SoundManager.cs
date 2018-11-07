using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

namespace SoundEngine
{
    public class SoundManager : MonoBehaviour
    {

        const string dllName = "FMOD Controller";

        [DllImport(dllName)]
        public static extern void initFMOD();

        [DllImport(dllName)]
        public static extern void update(/*float dt, int channel, bool _playing*/);

        [DllImport(dllName)]
        public static extern void setPosition(float _x, float _y, float _z, int channel);

        [DllImport(dllName)]
        public static extern void setVelocity(float _x, float _y, float _z, int channel);

        [DllImport(dllName)]
        public static extern void setVolume(float _volume, int channel);

        [DllImport(dllName)]
        public static extern void set3D(int channel);

        [DllImport(dllName)]
        public static extern void setMono(int channel);

        [DllImport(dllName)]
        public static extern void setLoop(int channel, bool looping);

        [DllImport(dllName)]
        public static extern bool createSound(string soundFile, int channel);

        [DllImport(dllName)]
        public static extern void setPlaying(bool temp, int channel);

        [DllImport(dllName)]
        public static extern bool getPlaying(int _channel);

        [DllImport(dllName)]
        public static extern void setListenerPos(float _x, float _y, float _z);

        [DllImport(dllName)]
        public static extern void setListenerVel(float _x, float _y, float _z);

        [DllImport(dllName)]
        public static extern void setListenerUp(float _x, float _y, float _z);

        [DllImport(dllName)]
        public static extern void setListenerForward(float _x, float _y, float _z);

        [DllImport(dllName)]
        public static extern void cleanFMOD();

        // Use this for initialization
        void Start()
        {
            initFMOD();
            setListenerUp(0f, 1f, 0f);
        }

        // Update is called once per frame
        void Update()
        {
            update();
        }

        void OnDestroy()
        {
            cleanFMOD();
        }
    }
}