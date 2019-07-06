using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{    
    public GameObject firstCloseMenuCanvas;
    public GameObject finalCloseMenuCanvas;
    public GameObject finalCloseMenuTimer;
    private CanvasGroup canvasGroup;
    private float targetTime;
    
    public void Start(){
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    public void Update(){
        //Call function to countdown the final close menu inactivity timer
        countDownTimer();
    }
    public void startCourse(string sceneName){
        //Load scene with given scenename
        SceneManager.LoadScene(sceneName);
    }

    public void destroyObjects(){
        //Destroy GamObjects with tag KeepAlive
        Destroy (GameObject.FindWithTag("KeepAlive"));
    }

    public void showCloseCanvas(){
        //Show the final close menu screen
        finalCloseMenuCanvas.SetActive(true);

        //Hide the initial close menu button
        // firstCloseMenuCanvas.transform.localScale = new Vector3(0, 0, 0);
        canvasGroup.alpha = 0f;

        //Start the function to hide the close menu screen again
        StartCoroutine(hideCloseCanvas());
    }

    IEnumerator hideCloseCanvas(){
        //Set the timer to 6 seconds
        targetTime = 6;
        
        //Wait for 6 seconds
        yield return new WaitForSeconds(6);

        //Hide the close menu canvas
        finalCloseMenuCanvas.SetActive(false);

        //Show the initial close menu button
        canvasGroup.alpha = 1f;
    }

    public void countDownTimer(){
        //Count down the timer
        targetTime -= Time.deltaTime;

        //Update the countdown text with the updated value of the timer
        finalCloseMenuTimer.GetComponent<Text>().text = "Dit venster sluit automatisch over " + Mathf.Round(targetTime) + " seconden.";
    }
}
