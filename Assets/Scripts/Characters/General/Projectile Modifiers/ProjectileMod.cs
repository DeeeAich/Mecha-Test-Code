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
        int roll = Random.Range(0, 100);
        if (roll > chance)
            return;

    }

    public static void AddOrUpdateMode(WeaStaEftChip chipWithMod, GameObject projectileOrWeapon)
    {



    }
}
