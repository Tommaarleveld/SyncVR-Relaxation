﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFragmentsAudio : MonoBehaviour {
	public AudioSource audioSource;
	public AudioClip[] clips;
	public int[] delays;
	public int sceneSwitchDelay;
	public string newSceneName;
    public GameObject cameraObject;
	private PlayBPMFeedback playBPMScript;


    // Use this for initialization
    void Start () {
		playBPMScript = GameObject.FindGameObjectWithTag("BPMScript").GetComponent<PlayBPMFeedback>();
	}

	public IEnumerator playAudioSequentially()
	{
		yield return null;

		//Loop through each AudioClip
		for (int i = 0; i < clips.Length; i++)
		{
			// Assign current AudioClip to audiosource
			audioSource.clip = clips[i];

			//Check the delaytime, if its longer than 22 seconds play the feedback audio
			if (delays[i] >= 22){
				//Wait for the first half of the delay	
				yield return new WaitForSeconds(delays[i] / 2);

				//Play the general feedback audio
				playBPMScript.giveGeneralFeedback();
					while (playBPMScript.audioSource.isPlaying){
						yield return null;
					}

				//Wait for the second half of the delay
				yield return new WaitForSeconds(delays[i] / 2);
			}
			else{
				yield return new WaitForSeconds(delays[i]);
			}
			
			audioSource.Play();

			//Wait for the audioclip to finish playing
			while (audioSource.isPlaying)
			{
				yield return null;
			}
			
		}

		if(this.newSceneName != null){
			yield return new WaitForSeconds(sceneSwitchDelay);
			SceneManager.LoadScene(newSceneName);
		}
		
	}
}
