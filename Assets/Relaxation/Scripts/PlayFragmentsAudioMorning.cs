using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFragmentsAudioMorning : MonoBehaviour {
	public AudioSource adSource;
	public AudioClip[] Clips;
	public int[] Delays;
	public GameObject nextFragment;

	// Use this for initialization
	void Start () {
		if (this.name == "FRAGMENT14") {
			StartCoroutine(playAudioSequentially());
		}
	}

	public void PlayAudio() {
		StartCoroutine(playAudioSequentially());
	}

	IEnumerator playAudioSequentially()
	{
		yield return null;

		//1.Loop through each AudioClip
		for (int i = 0; i < Clips.Length; i++)
		{
			//2.Assign current AudioClip to audiosource
			adSource.clip = Clips[i];

			if (i == 0) {
				yield return new WaitForSeconds(Delays[i]);
				adSource.Play();
			}
			else if (i == 1) {
				yield return new WaitForSeconds(Delays[i]);
				adSource.Play();

                //Switch back to main scene steps:
                //1. Enter main scene name in SceneManager.GetSceneByName string on line 48.
                //2. Enter main scene name in SceneManager.LoadScene string on line 54.
                if (this.name == "FRAGMENT18" && SceneManager.GetActiveScene() != SceneManager.GetSceneByName(""))
                {
                    //Delay after last audio file before unloading the scene.
                    yield return new WaitForSeconds(5);

                    //Load the main scene and unload the active scene.
                    SceneManager.LoadScene("");
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

                }
            }
			else if (i == 2) {
				yield return new WaitForSeconds(Delays[i]);
				adSource.Play();
            }
			else if (i == 3) {
				yield return new WaitForSeconds(Delays[i]);
				adSource.Play();
			}
			else if (i == 4) {
                yield return new WaitForSeconds(Delays[i]);
                adSource.Play();
            }
			else if (i == 5) {
				yield return new WaitForSeconds(Delays[i]);
				adSource.Play();
			}

			//3.Play Audio
			

			//4.Wait for it to finish playing
			while (adSource.isPlaying)
			{
				yield return null;
			}

			//5. Go back to #2 and play the next audio in the adClips array
		}
		if (nextFragment) { 
			nextFragment.GetComponent<PlayFragmentsAudioMorning>().PlayAudio();
		}
		
	}
}
