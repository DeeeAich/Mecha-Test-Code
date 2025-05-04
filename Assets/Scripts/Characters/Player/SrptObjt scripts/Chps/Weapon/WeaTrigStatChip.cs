using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerStatChip", menuName = "Player/Chip/Weapon/Trigger/Stat")]
public class WeaTrigStatChip : WeaponTriggerChip
{

    public WeaponStats chipStats;

    public float changeTime;
    private bool active = false;

    public override void TriggerActivate(Weapon weapon)
    {
        Debug.Log("Triggering ability");
        weapon.TempStatsAdd(chipStats, changeTime);
        
    }

}
