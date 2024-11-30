using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[CreateAssetMenu(fileName = "NewStatusEffectChip", menuName = "Player/Chip/StatusEffectChip")]
public class WeaStaEftChip : WeaponChip
{
    public enum StatusType
    {
        Critical, Burn,
        Hack, ShortCircuit
    }
    [Header("Status Adding")]

    public List<ChipClasses.StatusInfo> AddStatuses = new();

}

