using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayBPMFeedback : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> generalFeedbackClips;
    public List<AudioClip> BPMFeedbackClips;
    private AudioClip clipToPlay;

    public void giveGeneralFeedback(){
        int index = Random.Range(0, generalFeedbackClips.Count);
        clipToPlay = generalFeedbackClips[index];
        audioSource.clip = clipToPlay;
        audioSource.Play();
    }

    public void giveBPMFeedback(){
        int index = Random.Range(0, BPMFeedbackClips.Count);
        clipToPlay = BPMFeedbackClips[index];
        audioSource.clip = clipToPlay;
        audioSource.Play();
    }
}
