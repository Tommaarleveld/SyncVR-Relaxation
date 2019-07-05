using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPartManager : MonoBehaviour
{
    public List<PlayFragmentsAudio> audioParts;
    IEnumerator Start()
    {
        yield return null;

        //Call the function playAudioSequentially(), from the PlayFragmentsAudio.cs, for each audiopart.
        for (int i = 0; i < audioParts.Count; i++){
            yield return StartCoroutine(audioParts[i].playAudioSequentially());
        }
    }
}
