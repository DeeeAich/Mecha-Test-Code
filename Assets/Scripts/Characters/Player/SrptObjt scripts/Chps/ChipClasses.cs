using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class StatusInfo
{
    public WeaStaEftChip.StatusType statusType;
    [Header("If Applicable to the Status")]
    public float effectChance;
    public float effectDamage;
    public float effectTime;

}


[Serializable]
public class BodyStats
{
    public float health, armour, shield, shieldRegen;
    public bool percentage;

    public void AddStats(BodyStats stats)
    {
        if (percentage)
        {

            stats.health *= 1 + health / 100;
            stats.shieldRegen *= 1 + shieldRegen / 100;
            stats.armour *= armour / 100 + 1;
            stats.shield *= shield / 100 + 1;
        }
        else
        {
            stats.health += health;
            stats.shieldRegen += shieldRegen;
            stats.armour += armour;
            stats.shield += shield;
        }
    }
}

[Serializable]
public class LegStats
{

    public float speed, acceleration, turnSpeed;
    public bool percentage;

    public void AddStats(LegStats stats)
    {

        if (percentage)
        {
            stats.speed *= 1 + speed / 100;
            stats.acceleration *= 1 + acceleration / 100;
            stats.turnSpeed *= 1 + turnSpeed / 100;

        }
        else
        {
            stats.speed += speed;
            stats.acceleration += acceleration;
            stats.turnSpeed += turnSpeed;
        }


    }

}

[Serializable]
public class WeaponStats
{

    public float attackSpeed, damage, ammoCount, shotCost;

}