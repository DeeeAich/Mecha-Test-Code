using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable] public struct EnemySpawnStruct
{
    public EnemyType EnemyType;
    public GameObject prefab;
    public GameObject spawnAnimationPrefab;
    public int spawnChance;
    public float difficulty;
}

[Serializable] public enum EnemyType
{
    GunBoid,
    BomberDrone,
    BlueSpiderMech,
    ShieldDrone,
    MissileTank,
    Overseer,
    OverseerDrone,
    TeslaBot
}

[CreateAssetMenu(fileName = "Enemy Pool", menuName = "ScriptableObjects/Level Scriptables/Enemy Pool")]
public class EnemyPoolScriptable : ScriptableObject
{
    [FormerlySerializedAs("Enemies")] [Header("Enemies")]
    public EnemySpawnStruct[] enemies;

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
