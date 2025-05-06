using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Loot Pool", menuName = "ScriptableObjects/Level Scriptables/Loot Pool")]
public class LootPoolScriptable : ScriptableObject
{

    
    [FormerlySerializedAs("Chips")] [Header("Chips")]
    public PlayerPickup[] WeaponChips;
    public PlayerPickup[] BodyChips;

    [Header("Weapons")]
    public PlayerPickup[] Weapons;

    [Header("Ordinance")]
    public PlayerPickup[] Ordinance;

    [Header("Chassis")]
    public PlayerPickup[] Chassis;
}
