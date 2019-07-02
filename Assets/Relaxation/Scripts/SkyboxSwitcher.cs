using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using OVR;

public class SkyboxSwitcher : MonoBehaviour
{

    public List<Material> skyBoxes;
    public List<VideoPlayer> videoPlayerList;

    public List<RenderTexture> textureList;
    public List<int> skyBoxdelays;
    float skyboxExposure { get; set; }
    private float fadeDuration = 2.0f;

    void Start()
    {
      StartCoroutine(changeSkybox());  
    }

    IEnumerator changeSkybox(){
      yield return null;

    for (int i = 0; i < skyBoxes.Count; i++)
		{
			//Set skybox from the list skyBoxes.
			RenderSettings.skybox = skyBoxes[i];

      //Set the exposure to 0 so that the fading effect can be smooth.
      RenderSettings.skybox.SetFloat("_Exposure", 0.0f);

      //Set the render texture for the video to be played on.
      videoPlayerList[i].targetTexture = textureList[i];

      //Prepare the video to be played
      videoPlayerList[i].Prepare();

      //Play the video.
      videoPlayerList[i].Play();

      //Fade in with exposure
      StartCoroutine(Fade(0.0f, 1.0f));

      //Wait for delay
			yield return new WaitForSeconds(skyBoxdelays[i]);

      //Check if the video is the last one in the list. If so, keep playing the video.
      if(i != skyBoxes.Count -1){
        //Fade out with exposure and wait for animation to finish.
        StartCoroutine(Fade(1.0f, 0.0f));
        yield return new WaitForSeconds(fadeDuration);

        //Stop the video so it doesnt play in the background.
        videoPlayerList[i].Stop();
      }
		}
  }

  IEnumerator Fade(float startAlpha, float endAlpha){
    yield return null;
  
    float elapsedTime = 0.0f;
    //Check if the elapsed time doesn't exceed the duration of the fading.
     while (elapsedTime < fadeDuration){
       //During the duration of the fade gradually change the value of the skybox exposure according to the "startAlpha" and "endAlpha" parameters.
       elapsedTime += Time.deltaTime;
       skyboxExposure = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeDuration));
       RenderSettings.skybox.SetFloat("_Exposure", skyboxExposure);
       yield return new WaitForEndOfFrame();
     }
  }
}
