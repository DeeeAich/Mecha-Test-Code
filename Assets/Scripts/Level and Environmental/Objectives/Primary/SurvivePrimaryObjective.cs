using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivePrimaryObjective : Objective
{
    public float timer = 30f;
    private TMP_Text uiText;

    private void Start()
    {
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
                WaveSpawner[] spawners = room.waveSpawners;

                for (int i = 0; i < spawners.Length; i++)
                {
                    for (int j = 0; j < spawners[i].spawnedEnemies.Count; j++)
                    {
                        if (spawners[i].spawnedEnemies[j].TryGetComponent(out Health health))
                        {
                            health.TriggerDeath();
                        }
                    }

                    spawners[i].spawning = false;
                }
                
                TriggerComplete();
            }
            
            uiText.text = "Survive: " + Mathf.CeilToInt(timer);
        }
    }
}
