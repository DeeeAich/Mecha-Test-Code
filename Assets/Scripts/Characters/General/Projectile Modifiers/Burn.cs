using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        base.AttemptApply(target);

        target.GetComponent<IBurnable>().Burn(damage, chance, (int)duration * 4);
    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {
        if (statusInfo.statusType != WeaStaEftChip.StatusType.Burn)
            return;


        chance += statusInfo.effectChance;

        damage = damage < statusInfo.effectDamage || damage == 0 ?
            statusInfo.effectDamage : damage;
        duration = duration < statusInfo.effectTime || duration == 0 ?
            statusInfo.effectTime : duration;

    }

}
