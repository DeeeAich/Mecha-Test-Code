using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Info", menuName = "ScriptableObjects/Level Scriptables/Level Info")]
public class LevelScriptable : ScriptableObject
{
    public string levelName;

    public RoomPoolScriptable roomPool;
    public EnemyPoolScriptable enemyPool;
    public LootPoolScriptable[] lootPools;
    
    [Header("Images for display on things like doors and crates")]
    public GameObject pickupPrefab; 
    public Sprite weaponLootImage;
    public Sprite weaponChipLootImage;
    public Sprite chassisChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
