using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct LootPickupStruct
{
    public string lootName;
    public pickupType PickupType;
    public string lootDescription;
    
    public int spawnChance;
    public int rarity;

    public GameObject itemReference;
    public ScriptableObject ItemScriptableReference;
}

[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{
    public GameObject pickupPrefab; 
    
    [Header("Chips")]
    public LootPickupStruct[] Chips;

    [Header("Weapons")]
    public LootPickupStruct[] Weapons;

    [Header("Ordinance")]
    public LootPickupStruct[] Ordinance;

    [Header("Chassis")]
    public LootPickupStruct[] Chassis;
    
    [Header("Images for display on things like doors and crates")]
    public Sprite weaponLootImage;
    public Sprite combatChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
