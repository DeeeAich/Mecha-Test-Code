using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExterminatePrimaryObjective : Objective
{
    private int waveSpawnersComplete;
    private WaveSpawner[] WaveSpawners;

    private void Start()
    {
        WaveSpawners = GetComponents<WaveSpawner>();
    }

    private void FixedUpdate()
    {
        if (!isComplete)
        {
            waveSpawnersComplete = 0;
            for (int i = 0; i < WaveSpawners.Length; i++)
            {
                if (WaveSpawners[i].isComplete)
                {
                    waveSpawnersComplete++;
                }
            }

            if (waveSpawnersComplete == WaveSpawners.Length)
            {
                isComplete = true;
                onComplete.Invoke();
            }
        }
    }
}
