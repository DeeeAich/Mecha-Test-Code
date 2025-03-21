using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public enum SpawnType
{
    Standard, MiniBoss, Boss
}


public class WaveSpawner : MonoBehaviour
{
    public bool spawning;
    public bool isComplete;
    public int currentWave;
    public int enemiesKilled;
    public int totalEnemiesToSpawn;

    [Header("Settings")]
    public bool looping;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int remainingEnemiesToTriggerNextWave = 2;
    
    [SerializeField] private SpawnType[] enemyTypes = new [] {SpawnType.Standard};
    
    [Header("If unset, these will be randomly generated within fair ranges")]
    [SerializeField] private int[] waves;
    [SerializeField] private List<EnemySpawnStruct> enemiesToSpawn;

    [Header("References")]
    [SerializeField] private EnemySpawnPoint[] spawnPoints;
    [SerializeField] private GameObject enemySpawnPrefab;

    [Header("Internal References")]
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public List<GameObject> incomingEnemySpawners = new List<GameObject>();
    
    public UnityEvent onComplete;

    private List<EnemySpawnStruct> spawnableEnemies;

    private int enemyToSpawnIndex;
    private Random seededRandom;
    private float waveSpawnCooldown = 3;
    private float waveSpawnCooldownTimer;
    
    private void Awake() // creates enemy spawn pool
    {
        seededRandom = new Random(LevelGenerator.instance.seededRandom.Next());
        
        if (waves.Length == 0)
        {
            Debug.LogWarning("No waves set, not implemented");
        }

        spawnableEnemies = new List<EnemySpawnStruct>();
        if(enemyTypes.Contains(SpawnType.Standard)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.standardEnemies);
        if(enemyTypes.Contains(SpawnType.MiniBoss)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.miniBosses);
        if(enemyTypes.Contains(SpawnType.Boss)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.bosses);
    }

    private void Start() // finds spawn points
    {
        if (spawnPoints.Length == 0)
        {
            spawnPoints = LevelGenerator.instance.currentRoom.GetComponent<Room>().enemySpawnPoints;
        }
        if(spawnOnStart) LevelGenerator.instance.currentRoom.GetComponent<Room>().onStartRoom.AddListener(delegate { StartSpawning(); });
    }

    public void StartSpawning() // selects which enemies will be spawned
    {
        if (waves == null || waves.Length == 0)
        {
            waves = new int[seededRandom.Next(2, 4)];

            for (int i = 0; i < waves.Length; i++)
            {
                waves[i] = seededRandom.Next(remainingEnemiesToTriggerNextWave, spawnPoints.Length);
            }
        }
        
        if (enemiesToSpawn == null || enemiesToSpawn.Count == 0)
        {
            enemiesToSpawn = new List<EnemySpawnStruct>();

            for (int i = 0; i < waves.Length; i++)
            {
                int totalSpawnChance = 0;

                for (int j = 0; j < spawnableEnemies.Count; j++)
                {
                    totalSpawnChance += spawnableEnemies[j].spawnChance;
                }


                for (int j = 0; j < waves[i]; j++)
                {
                    int rand = seededRandom.Next(0, totalSpawnChance);
                    float currentChance = 0;
                    
                    for (int k = 0; k < spawnableEnemies.Count; k++)
                    {
                        currentChance += spawnableEnemies[k].spawnChance;
                        if (currentChance > rand)
                        {
                            enemiesToSpawn.Add(spawnableEnemies[k]);
                            break;
                        }
                    }
          
                }
            }
        }

        totalEnemiesToSpawn = enemiesToSpawn.Count;

        spawning = true;
        waveSpawnCooldownTimer = 0;
        currentWave = 0;
    }

    public void StopSpawning()
    {
        isComplete = true;
        
        if (spawnedEnemies != null && spawnedEnemies.Count > 0)
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if(spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
            }
        }// clears null references from list

        if (incomingEnemySpawners != null && incomingEnemySpawners.Count > 0)
        {
            for (int i = 0; i < incomingEnemySpawners.Count; i++)
            {
                if(incomingEnemySpawners[i] == null) incomingEnemySpawners.RemoveAt(i);
            }
        }// clears null references from list


        for (int i = 0; i < incomingEnemySpawners.Count; i++)
        {
            Destroy(incomingEnemySpawners[i]);
        }
    }

    private void FixedUpdate()
    {
        if (spawning && !isComplete)
        {
            if (spawnedEnemies != null && spawnedEnemies.Count > 0)
            {
                for (int i = 0; i < spawnedEnemies.Count; i++)
                {
                    if(spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
                }
            }// clears null references from list

            if (incomingEnemySpawners != null && incomingEnemySpawners.Count > 0)
            {
                for (int i = 0; i < incomingEnemySpawners.Count; i++)
                {
                    if(incomingEnemySpawners[i] == null) incomingEnemySpawners.RemoveAt(i);
                }
            }// clears null references from list
            
            if (currentWave == waves.Length && (spawnedEnemies == null || spawnedEnemies.Count == 0) && (incomingEnemySpawners == null || incomingEnemySpawners.Count == 0))
            {
                if (looping)
                {
                    currentWave = 0;
                    return;
                }
                onComplete.Invoke();
                isComplete = true;
                spawning = false;
                return;
            }
            
            if (waveSpawnCooldownTimer <= 0)
            {
                if ((spawnableEnemies == null || spawnedEnemies.Count <= remainingEnemiesToTriggerNextWave) && currentWave < waves.Length)
                {
                    SpawnWave(waves[currentWave]);
                }
            }
            else
            {
                waveSpawnCooldownTimer -= Time.fixedDeltaTime;
            }
        }
    }

    private void SpawnWave(int waveSize)
    {

        if (waveSize > spawnPoints.Length) waveSize = spawnPoints.Length;
        if (waveSize > enemiesToSpawn.Count - enemyToSpawnIndex) waveSize = enemiesToSpawn.Count - enemyToSpawnIndex;

        List<EnemySpawnPoint> availableSpawns = spawnPoints.ToList();

        for (int i = 0; i < waveSize; i++)
        {
            EnemySpawnPoint spawnPoint = availableSpawns[seededRandom.Next(0, availableSpawns.Count)];
            GameObject newSpawn = Instantiate(enemySpawnPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            availableSpawns.Remove(spawnPoint);
            
            newSpawn.transform.SetParent(transform);
            newSpawn.GetComponent<EnemySpawn>().enemyToSpawn = enemiesToSpawn[enemyToSpawnIndex];
            newSpawn.GetComponent<EnemySpawn>().waveSpawner = this;
            incomingEnemySpawners.Add(newSpawn);
            
            enemyToSpawnIndex++;
            if (looping && enemyToSpawnIndex == enemiesToSpawn.Count)
            {
                enemyToSpawnIndex = 0;
            }
        }
        
        waveSpawnCooldownTimer = waveSpawnCooldown;
        currentWave++;
    }
}
