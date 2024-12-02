using ChipClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMod : MonoBehaviour
{

    public float duration;
    public float chance;
    public float damage;

    public virtual void AttemptApply(GameObject target)
    {

    }

    public virtual void AddOrUpdateMode(StatusInfo statInfo)
    {

        duration += statInfo.effectTime;
        chance += statInfo.effectChance;
        damage += statInfo.effectDamage;

    }

    public virtual float AdditiveDamage(float baseDamage)
    {
        return baseDamage;
    }
}
