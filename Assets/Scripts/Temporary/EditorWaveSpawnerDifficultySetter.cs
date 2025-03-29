using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorWaveSpawnerDifficultySetter : MonoBehaviour
{
    [SerializeField] private bool EditorSetWaveSpawnersDifficulties;
    [SerializeField] private EnemyPoolScriptable enemyPool;

    
    private void OnDrawGizmos()
    {
        if (EditorSetWaveSpawnersDifficulties)
        {
            EditorSetWaveSpawnersDifficulties = false;
            
            Debug.LogWarning("Setting Difficulties, please stand by");

            List<GameObject> allWaveSpawners = new List<GameObject>();
            allWaveSpawners.AddRange(enemyPool.standardWaveSpawners);
            allWaveSpawners.AddRange(enemyPool.rareWaveSpawners);
            
            allWaveSpawners.AddRange(enemyPool.survivalWaveSpawners);
            allWaveSpawners.AddRange(enemyPool.captureWaveSpawners);
            allWaveSpawners.AddRange(enemyPool.exterminateWaveSpawners);

            allWaveSpawners.AddRange(enemyPool.miniBossWaveSpawners);
            allWaveSpawners.AddRange(enemyPool.bossWaveSpawners);

            for (int i = 0; i < allWaveSpawners.Count; i++)
            {
                WaveSpawner spawner = allWaveSpawners[i].GetComponent<WaveSpawner>();

                if (spawner.enemiesToSpawn != null && spawner.enemiesToSpawn.Count > 0)
                {
                    spawner.difficulty = 0;
                    int enemiesChecked = 0;
                    float difficulty = 0;

                    for (int j = 0; j < enemyPool.enemies.Length; j++)
                    {
                        if (spawner.enemiesToSpawn.Contains(enemyPool.enemies[j].EnemyType) && enemyPool.enemies[j].difficulty > spawner.difficulty)
                        {
                            enemiesChecked++;
                            difficulty = enemyPool.enemies[j].difficulty;
                            spawner.difficulty = difficulty;
                        }
                    }
                    
                    print("Enemies Checked = " + enemiesChecked + ", Difficulty set to " + difficulty);
                }
                else if(spawner.spawnableEnemyTypes != null && spawner.spawnableEnemyTypes.Count > 0)
                {
                    spawner.difficulty = 0;
                    int enemiesChecked = 0;
                    float difficulty = 0;
                    
                    for (int j = 0; j < enemyPool.enemies.Length; j++)
                    {
                        if (spawner.spawnableEnemyTypes.Contains(enemyPool.enemies[j].EnemyType) && enemyPool.enemies[j].difficulty > spawner.difficulty)
                        {
                            enemiesChecked++;
                            difficulty = enemyPool.enemies[j].difficulty;
                            spawner.difficulty = difficulty;
                        }
                    }
                    
                    print("Enemies Checked = " + enemiesChecked + ", Difficulty set to " + difficulty);
                }
            }
            
            Debug.LogWarning("Difficulties set");
        }
    }
}
