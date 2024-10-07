using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survive : Objective
{
    public float timer;
    
    private void FixedUpdate()
    {
        if (!isComplete)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                isComplete = true;
                onComplete.Invoke();
                WaveSpawner[] spawners = room.GetComponentsInChildren<WaveSpawner>();

                for (int i = 0; i < spawners.Length; i++)
                {
                    for (int j = 0; j < spawners[i].spawnedEnemies.Count; j++)
                    {
                        spawners[i].spawnedEnemies[j].GetComponent<Health>().TriggerDeath();
                    }
                }
            }
            
        }
    }
}
