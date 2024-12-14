using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCircuit : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        base.AttemptApply(target);

        target.GetComponent<IShortCircuitable>().ShortCircuit(chance, duration);
    }

    public override void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {

        if (statusInfo.statusType != WeaStaEftChip.StatusType.ShortCircuit)
            return;

        print("Adding to mod");

        chance += statusInfo.effectChance;
        if (duration < statusInfo.effectTime)
            duration = statusInfo.effectTime;

    }

}
