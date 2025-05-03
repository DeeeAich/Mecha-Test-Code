<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        if (chance < 1)
            return;

        target.GetComponent<IHackable>().Hack(damage, chance, duration);
    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {
        if (statusInfo.statusType != WeaStaEftChip.StatusType.Hack)
            return;


        chance += statusInfo.effectChance;
        damage = statusInfo.effectDamage > damage ? statusInfo.effectDamage : damage;
        duration = duration < statusInfo.effectTime || damage == 0 ?
            statusInfo.effectTime : duration;

    }

}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        if (chance < 1)
            return;

        target.GetComponent<IHackable>().Hack(damage, chance, duration);
    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {
        if (statusInfo.statusType != WeaStaEftChip.StatusType.Hack)
            return;


        chance += statusInfo.effectChance;
        damage = statusInfo.effectDamage > damage ? statusInfo.effectDamage : damage;
        duration = duration < statusInfo.effectTime || damage == 0 ?
            statusInfo.effectTime : duration;

    }

}
>>>>>>> Stashed changes
