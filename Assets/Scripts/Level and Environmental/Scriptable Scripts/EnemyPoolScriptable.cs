using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct EnemySpawnStruct
{
    public string Name;
    public GameObject prefab;
    public GameObject spawnAnimationPrefab;
    public int spawnChance;
    public int rarity;
}

[CreateAssetMenu(fileName = "Enemy Pool", menuName = "ScriptableObjects/Level Scriptables/Enemy Pool")]
public class EnemyPoolScriptable : ScriptableObject
{
    [Header("Enemies")]
    public EnemySpawnStruct[] standardEnemies;
    public EnemySpawnStruct[] miniBosses;
    public EnemySpawnStruct[] bosses;

    public List<EnemySpawnStruct> allEnemies;

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
