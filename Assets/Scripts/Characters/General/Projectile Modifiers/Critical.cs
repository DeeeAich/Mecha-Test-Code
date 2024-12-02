using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Critical : ProjectileMod
{

    public override float AdditiveDamage(float baseDamage)
    {

        float rChance = Random.Range(0, 100);

        if (rChance > chance)
            return baseDamage;

        return baseDamage * damage;

    }

}
