using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogToCanvas : MonoBehaviour
{
    public Text outputText;
    public Text pointerOutputText;

    private ControllerInactivityDetector inactivityDetector;
    private OVRGazePointer gazePointer;

	public void Start ()
    {
        inactivityDetector = FindObjectOfType<ControllerInactivityDetector>();
        gazePointer = FindObjectOfType<OVRGazePointer>();
	}
	
	public void Update ()
    {
        outputText.text = "Current Angle: " + inactivityDetector.angleDiff + "\n\n";
        outputText.text += ("Inactive Time: " + (Time.time - inactivityDetector.lastActivityTime) + "\n\n");
        outputText.text += ("Current State: " + inactivityDetector.IsActive);

        pointerOutputText.text = "z: " + gazePointer.transform.position.z;
	}
}
