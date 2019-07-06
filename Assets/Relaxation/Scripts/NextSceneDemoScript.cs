using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneDemoScript : MonoBehaviour
{
    public string sceneName;
     private float timeToPress = 3.0f;
     private float pressedTimer = 0f;

    void Update()
    {
    
    if (OVRInput.Get(OVRInput.Button.Any)){
         pressedTimer += Time.deltaTime;
         
         if (pressedTimer > timeToPress )
         {
            nextScene();
         }
     }
    }

    public void nextScene(){
        //Load scene with given scenename
        SceneManager.LoadScene(sceneName);
        ;
    }
}
