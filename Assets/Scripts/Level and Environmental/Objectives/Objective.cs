using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum WaveSpawnerType
{
    Standard, Rare, Miniboss, Boss, Exterminate, Survival, Capture
}

public class Objective : MonoBehaviour
{
    public bool isComplete;

    public ObjectiveType objectiveType;
    public WaveSpawnerType[] waveSpawnerTypesToAttemptToSpawn = new WaveSpawnerType[] {WaveSpawnerType.Standard};
    
    public Room room;
    
    public UnityEvent onComplete;

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
                        possibleWaveSpawners.AddRange(LevelGenerator.instance.levelInfo.enemyPool.miniBOssWaveSpawners);
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

            WaveSpawner newWaveSpawner = Instantiate(possibleWaveSpawners[LevelGenerator.instance.seededRandom.Next(0, possibleWaveSpawners.Count)].GetComponent<WaveSpawner>());
            room.waveSpawners = new WaveSpawner[] {newWaveSpawner};
            Debug.Log( "Spawned Wave: " + newWaveSpawner.name);
        }

        for (int i = 0; i < room.waveSpawners.Length; i++)
        {
            room.waveSpawners[i].StartSpawning();
        }
    }
}
