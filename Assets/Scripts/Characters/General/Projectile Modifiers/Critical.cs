using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critical : ProjectileMod
{

    public int lastCrit = 0;

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

        lastCrit = (int)crit;

        return baseDamage * (damage * crit + 1);
        
    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {


        if (statusInfo.statusType != WeaStaEftChip.StatusType.Critical)
            return;

        chance += statusInfo.effectChance;
        damage *= (1 + statusInfo.effectDamage);

    }
}
