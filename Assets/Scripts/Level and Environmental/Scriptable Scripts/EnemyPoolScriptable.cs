using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct EnemySpawnStruct
{
    public EnemyType EnemyType;
    public GameObject prefab;
    public GameObject spawnAnimationPrefab;
    public int spawnChance;
    public int difficulty;
}

[Serializable] public enum EnemyType
{
    GunBoid,
    BomberDrone,
    BlueSpiderMech,
    ShieldDrone,
    Overseer
}

[CreateAssetMenu(fileName = "Enemy Pool", menuName = "ScriptableObjects/Level Scriptables/Enemy Pool")]
public class EnemyPoolScriptable : ScriptableObject
{
    [Header("Enemies")]
    public EnemySpawnStruct[] standardEnemies;
    public EnemySpawnStruct[] bosses;

    [Header("Normal Wave Spawners")]
    public GameObject[] standardWaveSpawners;
    public GameObject[] rareWaveSpawners;
    public GameObject[] miniBossWaveSpawners;
    public GameObject[] bossWaveSpawners;

    [Header("Specific objective wave spawners")]
    public GameObject[] exterminateWaveSpawners;
    public GameObject[] survivalWaveSpawners;
    public GameObject[] captureWaveSpawners;
}
