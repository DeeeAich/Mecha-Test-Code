using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeToSpawn = 1.5f;
    public float timeToDespawn = 0.2f;
    private bool spawned;
    public WaveSpawner waveSpawner;

    private void Start()
    {
        if (LevelGenerator.instance.levelInfo.enemyPool.allEnemies.Contains(enemyToSpawn))
        {
            int index = LevelGenerator.instance.levelInfo.enemyPool.allEnemies.IndexOf(enemyToSpawn);
            if (index < LevelGenerator.instance.levelInfo.enemyPool.allEnemySpawnAnimationPrefabs.Length)
            {
                Instantiate(LevelGenerator.instance.levelInfo.enemyPool.allEnemySpawnAnimationPrefabs[index],
                    transform.position, transform.rotation);
            }

        }
        else
        {
            Debug.LogWarning("Spawned a spawner with no corresponding animation for it's enemy");
        }
        
    }

    private void FixedUpdate()
    {
        timeToSpawn -= Time.fixedDeltaTime;
        if (spawned)
        {
            timeToDespawn -= Time.fixedDeltaTime;
            if(timeToDespawn <= 0) Destroy(gameObject);
        }
        else if (timeToSpawn <= 0)
        {
            GameObject newEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
            newEnemy.transform.SetParent(transform.parent);
            if(waveSpawner != null) waveSpawner.spawnedEnemies.Add(newEnemy);
            spawned = true;
        }
    }
}
