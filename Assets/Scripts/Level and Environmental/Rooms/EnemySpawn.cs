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
            GameObject newEnemy = Instantiate(enemyToSpawn.prefab, transform.position, transform.rotation);
            newEnemy.transform.SetParent(transform.parent);
            if(waveSpawner != null) waveSpawner.spawnedEnemies.Add(newEnemy);
            spawned = true;
        }
    }
}
