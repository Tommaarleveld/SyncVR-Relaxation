using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssignToEventSystems : MonoBehaviour
{
	public void Start ()
    {
        FindObjectOfType<OVRInputModule>().rayTransform = transform;
        FindObjectOfType<OVRInputModule>().m_Cursor = GetComponentInChildren<OVRGazePointer>();
    }
}
