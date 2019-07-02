using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncVR.Util
{
    public static class SystemInfoUtil
    {
        private static string deviceID = "";
        private static string packageName = "";
        private static string storagePath = "";

        public static string DeviceID ()
        {
            if (deviceID == "")
            {
#if UNITY_ANDROID
                AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
                deviceID = jo.GetStatic<string>("SERIAL");
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                deviceID = SystemInfo.deviceName;
#else
                deviceID = ""
#endif
            }
            return deviceID;
        }

        public static string PackageName ()
        {
            if (packageName == "")
            {
                packageName = Application.identifier;
            }
            return packageName;
        }

        public static string PlatformName ()
        {
#if UNITY_ANDROID
            return "android";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return "windows";
#else
            return "";
#endif
        }

        public static string StoragePath ()
        {
            if (storagePath == "")
            {
#if UNITY_ANDROID

                AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
                storagePath = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
#else
                storagePath = Application.persistentDataPath;
#endif
            }

            return storagePath;
        }

        /**
         * Returns available storage space in MB
         */
        public static float GetAvailableStorage ()
        {
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            string path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");

            AndroidJavaObject stat = new AndroidJavaObject("android.os.StatFs", path);
            long blocks = stat.Call<long>("getAvailableBlocksLong");
            long blockSize = stat.Call<long>("getBlockSizeLong");

            return (blocks * blockSize) / 1000000f;
#else
            // 1 terabyte
            return 1000000;
#endif
        }
    }
}
