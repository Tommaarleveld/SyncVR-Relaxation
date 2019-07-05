using System.Collections;
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

			//Check the delaytime, if its longer than 20 seconds get the BPM state and play the feedback accordingly
			if (delays[i] >= 20 && ObservePulseData.getBPMState() != "NEUTRAL"){
				//Wait for the first half of the delay	
				yield return new WaitForSeconds(delays[i] / 2);

				if (ObservePulseData.getBPMState() == "HIGH"){
					Debug.Log("General feedback is supposed to start playing now");
					//Play the feedback and wait for it to finish playing.
					playBPMScript.giveGeneralFeedback();
					while (playBPMScript.audioSource.isPlaying){
					yield return null;
					}
				}
				else if(ObservePulseData.getBPMState() == "DESCENDING"){
					Debug.Log("General feedback is supposed to start playing now");
					//Play the feedback and wait for it to finish playing.
					playBPMScript.giveBPMFeedback();
					while (playBPMScript.audioSource.isPlaying){
					yield return null;
					}
				}

				//Wait for the second half of the delay
				yield return new WaitForSeconds(delays[i] / 2);
			}
			else{
				yield return new WaitForSeconds(delays[i]);
			}
			
			audioSource.Play();

			//4.Wait for it to finish playing
			while (audioSource.isPlaying)
			{
				yield return null;
			}
			//5. Go back to #2 and play the next audio in the clips array
		}

		if(this.newSceneName != null){
			yield return new WaitForSeconds(sceneSwitchDelay);
			SceneManager.LoadScene(newSceneName);
		}
		
	}
}
