using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTransformUpdater : MonoBehaviour
{
    private OVRInput.Controller activeController;

    IEnumerator Start ()
    {
        while (OVRInput.GetActiveController() == OVRInput.Controller.None)
        {
            yield return new WaitForSeconds(0.1f);
        }
        activeController = OVRInput.GetActiveController();
    }

    void Update ()
    {
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(activeController);
        Quaternion controllerRot = OVRInput.GetLocalControllerRotation(activeController);

        transform.localRotation = controllerRot;
    }
}
