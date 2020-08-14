using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void stopButton(){
        Time.timeScale=0;
    }

    public void continueButton(){
        Time.timeScale=1;
    }

    public void restartButton(){
        SceneManager.LoadScene(0);
    }
}
