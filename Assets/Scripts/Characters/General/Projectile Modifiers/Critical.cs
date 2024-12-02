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

        
        
        return baseDamage;

    }

}
