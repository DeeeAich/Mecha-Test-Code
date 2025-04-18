using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroCutsceneCamera : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float timer;
    public string minutes;
    public string hours;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float seconds = Mathf.RoundToInt(timer % 60);


        if (seconds < 10) { timerText.text = hours + ":" + minutes + ":" + "0" + seconds; }
        else
        {
            timerText.text = hours + ":" + minutes + ":" + seconds;
        }
       
    }

    public void changeTime()
    {
        minutes = "47";
        hours = "03";
    }
}
