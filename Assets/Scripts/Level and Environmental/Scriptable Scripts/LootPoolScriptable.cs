using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct LootPickupVariable
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
    public LootPickupVariable[] Chips;

    [Header("Weapons")]
    public LootPickupVariable[] Weapons;

    [Header("Ordinance")]
    public LootPickupVariable[] Ordinance;

    [Header("Chassis")]
    public LootPickupVariable[] Chassis;
    
    [Header("Images for display on things like doors and crates")]
    public Sprite weaponLootImage;
    public Sprite combatChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
