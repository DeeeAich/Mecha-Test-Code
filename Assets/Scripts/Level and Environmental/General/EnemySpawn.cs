using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public EnemySpawnStruct enemyToSpawn;
    public float timeToSpawn = 1.5f;
    public float timeToDespawn = 0.2f;
    private bool spawned;
    public WaveSpawner waveSpawner;

    private void Start()
    {
        Instantiate(enemyToSpawn.spawnAnimationPrefab, transform.position, transform.rotation);
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
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyToSpawn.prefab, transform.position, transform.rotation);
        newEnemy.transform.SetParent(transform.parent);
        
        if(GameGeneralManager.instance != null) newEnemy.GetComponent<EnemyStats>().difficulty = GameGeneralManager.instance.difficulty;
        
        if (waveSpawner != null)
        {
            waveSpawner.spawnedEnemies.Add(newEnemy);
            if (newEnemy.TryGetComponent(out Health enemyHealth))
            {
                WaveSpawner targetWaveSpawner = waveSpawner;
                enemyHealth.onDeath.AddListener(delegate
                {
                    targetWaveSpawner.enemiesKilled++;
                    //Debug.Log("Killed");
                });

           
            }
        }
        
        
        spawned = true;
    }
}
