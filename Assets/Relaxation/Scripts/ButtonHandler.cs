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
    private float targetTime;
    
    public void Start(){
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
        firstCloseMenuCanvas.GetComponent<Renderer>().enabled = false;

        //Start the function to hide the close menu screen again
        StartCoroutine(hideCloseCanvas());
    }

    IEnumerator hideCloseCanvas(){
        //Set the timer to 10 seconds
        targetTime = 6;
        
        //Wait for 10 seconds
        yield return new WaitForSeconds(6);

        //Hide the close menu canvas
        finalCloseMenuCanvas.SetActive(false);

        //Show the initial close menu button
        firstCloseMenuCanvas.GetComponent<Renderer>().enabled = true;
    }

    public void countDownTimer(){
        //Count down the timer
        targetTime -= Time.deltaTime;

        //Update the countdown text with the updated value of the timer
        finalCloseMenuTimer.GetComponent<Text>().text = "Dit venster sluit automatisch over " + Mathf.Round(targetTime) + " seconden.";
    }
}
