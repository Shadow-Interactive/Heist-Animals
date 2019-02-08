using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

namespace XBOX
{
    enum Buttons
    {
        A = 0x5800,
        B = 0x5801,
        X = 0x5802,
        Y = 0x5803,
        RB = 0x5804,
        LB = 0x5805,
        LTrig = 0x5806,
        RTrig = 0x5807,

        DPadUp = 0x5810,
        DPadDown = 0x5811,
        DPadLeft = 0x5812,
        DPadRight = 0x5813,
        Start = 0x5814,
        Select = 0x5815,
        L3 = 0x5816,
        R3 = 0x5817,

        LS_Up = 0x5820,
        LS_Down = 0x5821,
        LS_Right = 0x5822,
        LS_Left = 0x5823,
        LS_Up_Left = 0x5824,
        LS_Up_Rght = 0x5825,
        LS_Down_Right = 0x5826,
        LS_Down_Left = 0x5827,
    };

    public class XBoxInput : MonoBehaviour
    {
        const string DLLName = "XInput1_4 Wrapper";

        [DllImport(DLLName)]
        public static extern void initDLL();

        [DllImport(DLLName)]
        public static extern bool GetConnected(int index = 0);

        [DllImport(DLLName)]
        public static extern bool DownloadPackets(int num_controllers = 1);

        [DllImport(DLLName)]
        public static extern void UpdateController(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool GetKeyPressed(int _index, int _button);

        [DllImport(DLLName)]
        public static extern bool GetKeyReleased(int _index, int _button);

        [DllImport(DLLName)]
        public static extern bool GetKeyHeld(int _index, int _button);

        [DllImport(DLLName)]
        public static extern float GetLeftX(int _index = 0);

        [DllImport(DLLName)]
        public static extern float GetLeftY(int _index = 0);

        [DllImport(DLLName)]
        public static extern float GetRightX(int _index = 0);

        [DllImport(DLLName)]
        public static extern float GetRightY(int _index = 0);

        [DllImport(DLLName)]
        public static extern float GetLeftTrigger(int _index = 0);

        [DllImport(DLLName)]
        public static extern float GetRightTrigger(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool SetVibration(int _index, int lMotor, int rMotor);

        [DllImport(DLLName)]
        public static extern void cleanDLL();

        // Use this for initialization
        void Start()
        {
            initDLL();
        }

        // Update is called once per frame
        void Update()
        {
            DownloadPackets();

            if (!GetConnected())
            {
                //Debug.Log("Controller not Connected");
            }
            else
                UpdateController();
        }

        private void OnDestroy()
        {
            cleanDLL();
        }
    }
}
