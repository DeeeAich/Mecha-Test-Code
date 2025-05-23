using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum WaveSpawnerType
{
    Standard, Rare, Miniboss, Boss, Exterminate, Survival, Capture
}

public class Objective : MonoBehaviour
{
    public bool isComplete;
    
    public WaveSpawnerType[] waveSpawnerTypesToAttemptToSpawn = new WaveSpawnerType[] {WaveSpawnerType.Standard};
    
    public Room room;
    
    public UnityEvent onComplete;

    public Image progressBar;

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        
        if (room.waveSpawners == null || room.waveSpawners.Length == 0)
        {
            List<GameObject> possibleWaveSpawners = new List<GameObject>();

            for (int i = 0; i < waveSpawnerTypesToAttemptToSpawn.Length; i++)
            {
                switch (waveSpawnerTypesToAttemptToSpawn[i])
                {
                    case WaveSpawnerType.Standard:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.standardWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Rare:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.rareWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Miniboss:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.miniBossWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Boss:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.bossWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Exterminate:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.exterminateWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Survival:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.survivalWaveSpawners);
                        break;
                    
                    case WaveSpawnerType.Capture:
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.captureWaveSpawners);
                        break;
                }
            }

            if (possibleWaveSpawners.Count > 0)
            {
                WaveSpawner newWaveSpawner = Instantiate(possibleWaveSpawners[LevelGenerator.instance.seededRandom.Next(0, possibleWaveSpawners.Count)], transform.parent).GetComponent<WaveSpawner>();
                room.waveSpawners = new WaveSpawner[] {newWaveSpawner};
                Debug.Log( "Spawned Wave: " + newWaveSpawner.name);
            }
        }

        if (room.waveSpawners.Length > 0)
        {
            for (int i = 0; i < room.waveSpawners.Length; i++)
            {
                room.waveSpawners[i].gameObject.SetActive(true);
                room.waveSpawners[i].StartSpawning();
            }
        }

    }

    public void TriggerComplete()
    {
        print("Completed " + name);
        if (room.waveSpawners.Length > 0)
        {
            for (int i = 0; i < room.waveSpawners.Length; i++)
            {
                room.waveSpawners[i].StopSpawning(true);
            }
        }


        onComplete.Invoke();
        isComplete = true;
    }
}
