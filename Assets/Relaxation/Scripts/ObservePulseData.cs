// Last update: 2018-05-20  (by Dikra)

using UnityEngine;

using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

public class ObservePulseData : MonoBehaviour
{

    static int debug_idx = 0;
    private static string bpmState = "NEUTRAL";

    private int lastBPM;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(getFirebaseBPM());
    }

    void DebugLog(string str)
    {
        Debug.Log(str);
    }
    IEnumerator getFirebaseBPM()
    {
        Firebase firebase = Firebase.CreateNew("https://pulsesensor-a13e2.firebaseio.com/PulseData", "RyvmMbMFgX2vxBhbv2XQDlLZhZ1Y7NOFr9E0EmYG");

        // Get child node from firebase, if false then all the callbacks are not inherited.
        Firebase lastPulseReading = firebase.Child("BPM");

        // Make observer on "last update" time stamp
        FirebaseObserver observer = new FirebaseObserver(lastPulseReading, 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {
            // DebugLog("[OBSERVER] Last Pulsereading changed to: " + snapshot.Value<long>());
            int newBPM = unchecked((int)snapshot.Value<long>());

            if (lastBPM - newBPM >= 10){
                bpmState = "DESCENDING";
            }
            else if(newBPM >= 100){
                bpmState = "HIGH";
            }
            else{
                bpmState = "NEUTRAL";
            }

            lastBPM = unchecked((int)snapshot.Value<long>());
            DebugLog(bpmState);
        };
        observer.Start();
        DebugLog("[OBSERVER] FirebaseObserver on " + lastPulseReading.FullKey + " started!");

        // Unnecessarily skips a frame, really, unnecessary.
        yield return null;

        // Create a FirebaseQueue
        FirebaseQueue firebaseQueue = new FirebaseQueue(true, 3, 1f); // if _skipOnRequestError is set to false, queue will stuck on request Get.LimitToLast(-1).
    }

    public static string getBPMState()
    {
        return bpmState;
    }
}