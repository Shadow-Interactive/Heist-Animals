using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace OUTPUT_GENERATOR
{
    public class FileWriting : MonoBehaviour
    {
        const string DllName = "Singleton_Filewrite";

        [DllImport(DllName)]
        public static extern void initDLL(string filePath = "RunnerPositionData.shadow");

        [DllImport(DllName)]
        public static extern bool writeData(Vector3 data);

        [DllImport(DllName)]
        public static extern bool writeString(string data);

        [DllImport(DllName)]
        public static extern void clearDLL();

        private float timeCount;

        // Use this for initialization
        void Start()
        {
            initDLL();
            writeString("New Runner Sequence\n");
            timeCount = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 0.25)
            {
                writeData(GetComponent<Transform>().position);
                timeCount = 0.0f;
            }
        }

        //used for destruction of object
        private void OnDestroy()
        {
            writeString("\nEnd Runner Sequence");
            //clearDLL();
        }
    }
}