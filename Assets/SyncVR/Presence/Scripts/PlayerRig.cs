using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncVR.Presence
{
    public class PlayerRig : MonoBehaviour
    {
        public GameObject cameraRig;
        public GameObject controller;

        public void Start()
        {

        }

        public void SetUpDownRotation(float angle)
        {
            cameraRig.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            controller.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

    }
}