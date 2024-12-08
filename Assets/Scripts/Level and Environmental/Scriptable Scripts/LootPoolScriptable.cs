using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{
    public GameObject[] standardChips;
    public GameObject[] rareChips;
    public GameObject[] legendaryChips;

    public GameObject[] standardWeapons;
    public GameObject[] rareWeapons;
    public GameObject[] legendaryWeapons;

    public GameObject[] standardOrdinance;
    public GameObject[] rareOrdinance;
    public GameObject[] legendaryOrdinance;

    public GameObject[] standardChassis;
    
    public Sprite weaponLootImage;
    public Sprite combatChipLootImage;
    public Sprite ordinanceLootImage;
    public Sprite chassisLootImage;
}
