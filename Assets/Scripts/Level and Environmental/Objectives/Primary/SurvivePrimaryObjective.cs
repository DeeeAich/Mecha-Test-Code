using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivePrimaryObjective : Objective
{
    public float timer = 30f;
    private TMP_Text uiText;
    private float totalTime;

    private void Start()
    {
        totalTime = timer;
        uiText = GetComponentInChildren<TMP_Text>();
        for (int i = 0; i < room.waveSpawners.Length; i++)
        {
            room.waveSpawners[i].looping = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {

            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                TriggerComplete();
            }
            
            uiText.text = "Survive: " + Mathf.CeilToInt(timer);
            if(progressBar != null) progressBar.fillAmount = 1f - timer / totalTime;
        }
    }
}
