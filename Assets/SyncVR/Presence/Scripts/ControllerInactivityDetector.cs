using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInactivityDetector : MonoBehaviour
{
    [Tooltip ("In degrees of rotation per fixed update interval (0.02s).")]
    public float activityThreshold = 1.5f;

    [Tooltip ("How long there has to be no movement before controller is considered inactive.")]
    public float activityHysteresisPeriod = 2f;

    public Action ControllerBecameActive;
    public Action ControllerBecameInactive;

    public bool IsActive { get; private set; }
    public float lastActivityTime { get; private set; }
    public float angleDiff { get; private set; }

    private Vector3 previousForward = Vector3.zero;

    public void Start ()
    {
        IsActive = false;
    }

    public void FixedUpdate ()
    {
        Vector3 forward = transform.forward;
        angleDiff = Vector3.Angle(previousForward, forward);
        if (angleDiff > activityThreshold)
        {
            if (!IsActive && ControllerBecameActive != null)
            {
                ControllerBecameActive.Invoke();
            }
            IsActive = true;
            lastActivityTime = Time.time;
        }
        else
        {
            if (Time.time - lastActivityTime > activityHysteresisPeriod)
            {
                if (IsActive && ControllerBecameInactive != null)
                {
                    ControllerBecameInactive.Invoke();
                }
                IsActive = false;
            }
        }

        previousForward = forward;
    }
}
