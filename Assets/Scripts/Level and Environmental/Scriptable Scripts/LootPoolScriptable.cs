using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{
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
