using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

namespace SoundEngine
{

    public class SoundManager : MonoBehaviour
    {
        public static string soundPath;

        const string dllName = "FMOD Controller";

        [DllImport(dllName)]
        public static extern void initFMOD();
        
        [DllImport(dllName)]
        public static extern void update(/*float dt, int channel, bool _playing*/);
        
        [DllImport(dllName)]
        public static extern bool playSound(int channel, float dt);
        
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

        [DllImport(dllName)]
        public static extern void cleanChannels();
        
        // Use this for initialization
        void Start()
        {
            soundPath = Application.dataPath + "/Audio/";
            initFMOD();
            setListenerUp(0f, 1f, 0f);
            createSound(soundPath + "Sneaky-Beaky Like.mp3", 0);
            setVolume(0f, 0);
            setLoop(0, true);
            setMono(0);
            setPlaying(true, 0);
            //playSound(0, Time.deltaTime);
            
            SoundManager.createSound(SoundManager.soundPath + "ZapperFire.mp3", 1);
        }
        
        // Update is called once per frame
        void Update()
        {
            update();

            PlaySounds(0);
        }
        
        void OnDestroy()
        {
            cleanFMOD();
            cleanChannels();
        }

        public void PlaySounds(int _channel)
        {
            if (!getPlaying(0))
            {
                print("What the fuck???!!!\n");
            }
            else
            {
                setPlaying(true, _channel);
                //playSound(0, Time.deltaTime);
                print("Start playing again\n");
            }
        }
    }
}