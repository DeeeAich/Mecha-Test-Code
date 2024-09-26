using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeToSpawn = 1;
    public float timeToDespawn = 0.2f;
    private bool spawned;
    public WaveSpawner waveSpawner;
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
            if(waveSpawner != null) waveSpawner.spawnedEnemies.Add(newEnemy);
            spawned = true;
        }
    }
}
