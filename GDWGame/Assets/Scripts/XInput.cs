using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

namespace XBOX
{
    public class XInput : MonoBehaviour
    {
        const string DLLName = "XBoxInput DLL";

        [DllImport(DLLName)]
        public static extern void initXBOX();

        [DllImport(DLLName)]
        public static extern void getPackets(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getConnected(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getLeftStickXAxis(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getLeftStickYAxis(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getRightStickXAxis(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getRightStickYAxis(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getLeftTrigger(int _index = 0);

        [DllImport(DLLName)]
        public static extern float getRightTrigger(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonR3(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonL3(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonStart(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonSelect(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonLeft(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonRight(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonUp(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonDown(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonA(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonB(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonX(int _index = 0);

        [DllImport(DLLName)]
        public static extern bool getButtonY(int _index = 0);

        // Use this for initialization
        void Start()
        {
            initXBOX();
        }

        // Update is called once per frame
        void Update()
        {
            if(getConnected())
            {
                getPackets();

                if (getButtonB())
                {
                    Application.Quit();
                }
            }
        }
    }
}
