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
    public float health, armour;
    public bool percentage;

    public void AddStats(BodyStats stats)
    {
        if (percentage)
        {

            stats.health *= 1 + health / 100;
            stats.armour *= armour / 100 + 1;
        }
        else
        {
            stats.health += health;
            stats.armour += armour;
        }
    }

    public void SetStats(BodyStats stats)
    {

        health = stats.health;
        armour = stats.armour;

    }
}

[Serializable]
public class LegStatChange
{

    public float speed, acceleration, turnSpeed;
    public bool percentage;

    public void AddStats(LegStatChange stats)
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
public class DashStatChange
{
    [Tooltip("Add additional charges or remove")]
    public int dashCharges;
    [Tooltip("Add for slower, minus for faster")]
    public float dashTime;
    [Tooltip("Add distance or remove distance")]
    public float dashDistance;
    [Tooltip("Add to increase time, minus to decrease")]
    public float dashRecharge;

}

[Serializable]
public class WeaponStats
{

    public float attackSpeed = 1;
    public float minAttackSpeed = 0.2f;
    public float damage = 1;
    public float ammoCount = 1;
    public float shotCost = 0;
    public float reloadSpeed = 1;
    public float minReloadSpeed = 0.4f;

    public void AddStats(WeaponStats statsToUpdate)
    {

        statsToUpdate.attackSpeed -= attackSpeed;
        if (statsToUpdate.attackSpeed < statsToUpdate.minAttackSpeed)
            statsToUpdate.attackSpeed = statsToUpdate.minAttackSpeed;
        statsToUpdate.damage += damage;
        statsToUpdate.ammoCount += ammoCount;
        statsToUpdate.shotCost += shotCost;
        statsToUpdate.reloadSpeed -= reloadSpeed;
        if (statsToUpdate.reloadSpeed < statsToUpdate.minReloadSpeed)
            statsToUpdate.reloadSpeed = statsToUpdate.minReloadSpeed;

    }

    public void RemoveStats(WeaponStats statsToUpdate)
    {

        statsToUpdate.attackSpeed -= attackSpeed;
        statsToUpdate.damage -= damage;

    }

}