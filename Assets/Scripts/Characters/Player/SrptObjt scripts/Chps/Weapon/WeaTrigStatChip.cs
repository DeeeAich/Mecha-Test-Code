using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaTrigStatChip : WeaponTriggerChip
{

    public WeaponStats chipStats;

    public float changeTime;

    public override void TriggerActivate(Weapon weapon)
    {

        StartCoroutine(StatChangeTimer(weapon));

    }

    public IEnumerator StatChangeTimer(Weapon weapon)
    {

        weapon.TempStatsAdd(chipStats);

        yield return new WaitForSeconds(changeTime);

        weapon.TempStatsRemove(chipStats);

        yield return null;

    }

}
