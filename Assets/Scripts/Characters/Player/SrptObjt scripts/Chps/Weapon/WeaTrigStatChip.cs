using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerStatChip", menuName = "Player/Chip/Weapon/Trigger/Stat")]
public class WeaTrigStatChip : WeaponTriggerChip
{

    public WeaponStats chipStats;

    public float changeTime;
    private bool active = false;

    public override void ChipTriggerSetter(Weapon weapon)
    {

        base.ChipTriggerSetter(weapon);
    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {
        base.ChipTriggerUnsetter(weapon);
    }

    public override void TriggerActivate(Weapon weapon)
    {
        weapon.TempTimedStatsAdd(chipStats, changeTime);
        
    }

}
