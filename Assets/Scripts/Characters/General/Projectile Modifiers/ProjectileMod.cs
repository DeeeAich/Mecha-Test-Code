<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMod : MonoBehaviour, IMod
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

    public virtual void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {

        if (statusInfo.statusType.ToString() != GetType().ToString())
            return;

        duration = duration < statusInfo.effectTime || duration == 0 ? duration : statusInfo.effectTime;
        damage = damage < statusInfo.effectDamage || damage == 0 ? statusInfo.effectDamage : damage;
        chance += statusInfo.effectChance;

    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMod : MonoBehaviour, IMod
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

    public virtual void AddModifiers(StatusInfo statusInfo, bool percentage = false)
    {

        if (statusInfo.statusType.ToString() != GetType().ToString())
            return;

        duration = duration < statusInfo.effectTime || duration == 0 ? duration : statusInfo.effectTime;
        damage = damage < statusInfo.effectDamage || damage == 0 ? statusInfo.effectDamage : damage;
        chance += statusInfo.effectChance;

    }
}
>>>>>>> Stashed changes
