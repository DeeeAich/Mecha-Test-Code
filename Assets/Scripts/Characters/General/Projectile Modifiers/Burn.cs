using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : ProjectileMod
{

    public override void AttemptApply(GameObject target)
    {
        base.AttemptApply(target);

        //target.GetComponent<IBurnable>().Burn(damage, chance, durationc * 4);
    }

}
