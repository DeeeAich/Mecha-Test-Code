using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hack : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        base.AttemptApply(target);

        target.GetComponent<IHackable>().Hack(damage, chance, duration);
    }

}
