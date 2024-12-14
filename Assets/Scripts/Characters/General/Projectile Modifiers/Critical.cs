using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critical : ProjectileMod
{

    public override float AdditiveDamage(float baseDamage)
    {

        float crit = 0;

        float checkChance = chance;

        while (checkChance > 100)
        {
            checkChance -= 100;
            crit++;
        }

        if (Random.Range(0, 100) <= checkChance)
            crit++;
        
        
        return baseDamage * Mathf.Pow(damage, crit);

    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {


        if (statusInfo.statusType != WeaStaEftChip.StatusType.Critical)
            return;

        chance += statusInfo.effectChance;
        damage = damage < statusInfo.effectDamage || damage == 0 ?
            statusInfo.effectDamage : damage;

    }
}
