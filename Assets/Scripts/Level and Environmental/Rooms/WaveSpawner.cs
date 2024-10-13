using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public enum SpawnType
{
    Standard, Rare, MiniBoss, Boss
}
public class WaveSpawner : MonoBehaviour
{
    public bool spawning;
    public bool isComplete;
    public int currentWave;

    public bool looping;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int remainingEnemiesToTriggerNextWave = 2;
    
    public UnityEvent onComplete;
    [SerializeField] private GameObject enemySpawnPrefab;
    
    [SerializeField] private int[] waves;
    [SerializeField] private SpawnType[] enemyTypes = new [] {SpawnType.Standard};
    [SerializeField] private GameObject[] spawnPoints;

    public List<GameObject> spawnedEnemies;

    private List<GameObject> spawnableEnemies;
    private Random seededRandom;
    private float waveSpawnCooldown = 3;
    private float waveSpawnCooldownTimer;
    
    private void Awake()
    {
        seededRandom = new Random(LevelGenerator.instance.seededRandom.Next());
        
        if (waves.Length == 0)
        {
            Debug.LogWarning("No waves set, not implemented");
        }

        spawnableEnemies = new List<GameObject>();
        if(enemyTypes.Contains(SpawnType.Standard)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.standardEnemies);
        if(enemyTypes.Contains(SpawnType.Rare)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.rareEnemies);
        if(enemyTypes.Contains(SpawnType.MiniBoss)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.miniBosses);
        if(enemyTypes.Contains(SpawnType.Boss)) spawnableEnemies.AddRange(LevelGenerator.instance.levelInfo.enemyPool.bosses);
        
        
    }

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            spawnPoints = LevelGenerator.instance.currentRoom.GetComponent<Room>().enemySpawnPoints;
        }
        if(spawnOnStart) LevelGenerator.instance.currentRoom.GetComponent<Room>().onStartRoom.AddListener(delegate { StartSpawning(); });
    }

    public void StartSpawning()
    {
        spawning = true;
        SpawnWave(waves[0]);
        currentWave = 0;
    }

    private void FixedUpdate()
    {
        if (spawning)
        {
            if (currentWave == waves.Length - 1 && spawnedEnemies.Count == 0)
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
            
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if(spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
            }
            
            if (waveSpawnCooldownTimer <= 0)
            {
                if (spawnedEnemies.Count <= remainingEnemiesToTriggerNextWave && currentWave < waves.Length)
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

    private void SpawnWave(int wavSize)
    {
        if (wavSize > spawnPoints.Length) wavSize = spawnPoints.Length;

        List<GameObject> availableSpawns = spawnPoints.ToList();

        for (int i = 0; i < wavSize; i++)
        {
            GameObject spawnPoint = availableSpawns[seededRandom.Next(0, availableSpawns.Count)];
            GameObject newSpawn = Instantiate(enemySpawnPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            newSpawn.GetComponent<EnemySpawn>().enemyToSpawn = spawnableEnemies[seededRandom.Next(0, spawnableEnemies.Count)];
            newSpawn.GetComponent<EnemySpawn>().waveSpawner = this;
        }
        
        waveSpawnCooldownTimer = waveSpawnCooldown;
        currentWave++;
    }
}
