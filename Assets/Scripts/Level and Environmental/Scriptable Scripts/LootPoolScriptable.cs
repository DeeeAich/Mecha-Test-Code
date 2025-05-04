using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{
    public GameObject pickupPrefab; 
    
    [FormerlySerializedAs("Chips")] [Header("Chips")]
    public PlayerPickup[] WeaponChips;
    public PlayerPickup[] BodyChips;

    [Header("Weapons")]
    public PlayerPickup[] Weapons;

    [Header("Ordinance")]
    public PlayerPickup[] Ordinance;

    [Header("Chassis")]
    public PlayerPickup[] Chassis;
    
    [Header("Images for display on things like doors and crates")]
    public Sprite weaponLootImage;
    public Sprite weaponChipLootImage;
    public Sprite chassisChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
