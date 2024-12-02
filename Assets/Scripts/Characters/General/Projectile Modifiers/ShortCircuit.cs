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

}
