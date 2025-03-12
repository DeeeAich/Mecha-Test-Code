using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExterminatePrimaryObjective : Objective
{
    private int waveSpawnersComplete;
    private WaveSpawner[] WaveSpawners;

    private int progress;
    private float totalProgress;

    private void Start()
    {
        WaveSpawners = room.waveSpawners;

        totalProgress = 0;


    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {
            waveSpawnersComplete = 0;
            progress = 0;
            totalProgress = 0;
            
            for (int i = 0; i < WaveSpawners.Length; i++)
            {
                if (WaveSpawners[i].isComplete)
                {
                    waveSpawnersComplete++;
                }

                totalProgress += WaveSpawners[i].totalEnemiesToSpawn;
                progress += WaveSpawners[i].enemiesKilled;
            }

            if (progressBar != null) progressBar.fillAmount = progress / totalProgress;

            if (waveSpawnersComplete == WaveSpawners.Length)
            {
                TriggerComplete();
            }
        }
    }
}
