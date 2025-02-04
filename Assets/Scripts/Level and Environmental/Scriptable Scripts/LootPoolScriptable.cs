using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct LootPickupVariable
{
    public float spawnChance;
    public int rarity;
    
    public GameObject itemReference;
    public ScriptableObject ItemScriptableReference;
}

[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{
    /*
    
    public Gameobject pickupPrefab; 
    
    [Header("Chips")]
    public LootPickupVariable[] standardChips;
    public LootPickupVariable[] rareChips;
    public LootPickupVariable[] legendaryChips;

    [Header("Weapons")]
    public LootPickupVariable[] standardWeapons;
    public LootPickupVariable[] rareWeapons;
    public LootPickupVariable[] legendaryWeapons;

    [Header("Ordinance")]
    public LootPickupVariable[] standardOrdinance;
    public LootPickupVariable[] rareOrdinance;
    public LootPickupVariable[] legendaryOrdinance;

    [Header("Chassis")]
    public LootPickupVariable[] standardChassis;
    
    [Header("Images for display on things like doors and crates")]
    public Sprite weaponLootImage;
    public Sprite combatChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
    */

    [Header("Chips")]
    public GameObject[] standardChips;
    public GameObject[] rareChips;
    public GameObject[] legendaryChips;

    [Header("Weapons")]
    public GameObject[] standardWeapons;
    public GameObject[] rareWeapons;
    public GameObject[] legendaryWeapons;

    [Header("Ordinance")]
    public GameObject[] standardOrdinance;
    public GameObject[] rareOrdinance;
    public GameObject[] legendaryOrdinance;

    [Header("Chassis")]
    public GameObject[] standardChassis;
    
    [Header("Images for display on things like doors and crates")]
    public Sprite weaponLootImage;
    public Sprite combatChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
