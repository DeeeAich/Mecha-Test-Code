using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Pool", menuName = "ScriptableObjects/Level Scriptables/Enemy Pool")]
public class EnemyPoolScriptable : ScriptableObject
{
    [Header("Enemies")]
    public GameObject[] standardEnemies;
    public GameObject[] rareEnemies;
    public GameObject[] miniBosses;
    public GameObject[] bosses;

    public List<GameObject> allEnemies;
    public GameObject[] allEnemySpawnAnimationPrefabs;

    [Header("Normal Wave Spawners")]
    public GameObject[] standardWaveSpawners;
    public GameObject[] rareWaveSpawners;
    public GameObject[] miniBOssWaveSpawners;
    public GameObject[] bossWaveSpawners;

    [Header("Specific objective wave spawners")]
    public GameObject[] exterminateWaveSpawners;
    public GameObject[] survivalWaveSpawners;
    public GameObject[] captureWaveSpawners;
}
