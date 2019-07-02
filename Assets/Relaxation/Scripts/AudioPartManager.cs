using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPartManager : MonoBehaviour
{
    public List<PlayFragmentsAudio> audioParts;
    IEnumerator Start()
    {
        yield return null;

        for (int i = 0; i < audioParts.Count; i++){
            yield return StartCoroutine(audioParts[i].playAudioSequentially());
        }
    }
}
