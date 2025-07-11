﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using UnityEngine.Events;

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

    public void AddStats(BodyStats stats)
    {
        stats.health += health;
        stats.armour += armour;
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

    public void AddStats(LegStatChange stats)
    {

        stats.speed *= speed + 1;
        stats.acceleration *= acceleration + 1;
        stats.turnSpeed *= turnSpeed + 1;

    }

    public void RemoveStats(LegStatChange stats)
    {

        stats.speed /= speed + 1;
        stats.acceleration /= acceleration + 1;
        stats.turnSpeed /= turnSpeed + 1;

    }

}

[Serializable]
public class DashStatChange
{
    [Tooltip("Add additional charges or remove count")]
    public int dashCharges = 0;
    [Tooltip("Add for slower, minus for faster .")]
    public float dashTime = 1;
    [Tooltip("Add distance or remove distance .")]
    public float dashDistance = 1;
    [Tooltip("Add to increase time, minus to decrease .")]
    public float dashRecharge = 1;

    public void AddStats(DashStatChange stats)
    {

        stats.dashCharges += dashCharges;
        stats.dashTime *= 1 + dashTime;
        stats.dashDistance *= dashDistance + 1;
        stats.dashRecharge *= 1 - dashRecharge;

    }

    public void RemoveStats(DashStatChange stats)
    {

        stats.dashCharges -= dashCharges;
        stats.dashTime /= dashTime;
        stats.dashDistance /= dashDistance + 1;
        stats.dashRecharge /= dashRecharge;

    }

}

[Serializable]
public class WeaponStats
{

    public float attackSpeed = 1;
    public float damage = 1;
    public float ammoCount = 1;
    public int shotCost = 0;
    public float reloadSpeed = 1;
    public int piercing = 0;
    public float diviation = 1;

    public void AddStats(WeaponStats statsToUpdate)
    {

        statsToUpdate.attackSpeed *= attackSpeed + 1;
        statsToUpdate.damage *= damage + 1;
        statsToUpdate.ammoCount = ammoCount + 1;
        statsToUpdate.shotCost += shotCost;
        statsToUpdate.reloadSpeed *= reloadSpeed + 1;
        if (piercing <= -1)
            piercing = -1;
        else
            statsToUpdate.piercing += piercing;
        statsToUpdate.diviation *= 1 - diviation;


    }

    public void RemoveStats(WeaponStats statsToUpdate)
    {

        statsToUpdate.attackSpeed /= attackSpeed + 1;
        statsToUpdate.damage /= damage + 1;
        statsToUpdate.reloadSpeed /= reloadSpeed + 1;
        statsToUpdate.diviation /= 1 - diviation;

    }

}

[Serializable]
public class ChipEnums
{

    public enum Trigger
    {
        Damaged, OnKill,
        OnShot,OnRoomClear,
        OnHeal, moveStart,
        moveEnd, reload,
        constant, dashed,
        replenish, none
    }

}

[Serializable]
public class TriggerEvents
{

    public UnityAction healed, damaged,
        dashed, moveStart, moveEnd,
        killedLeft, killedRight,
        fireLeft, fireRight,
        reloadLeft, reloadRight,
        replenishLeft, replenishRight,
        roomClear, constant;

    public void ClearEvents()
    {
        healed = null;
        damaged = null;
        dashed = null;
        moveStart = null;
        moveEnd = null;
        killedLeft = null;
        killedRight = null;
        reloadLeft = null;
        reloadRight = null;
        replenishLeft = null;
        replenishRight = null;
        roomClear = null;
        constant = null;
    }

}

[Serializable]
public class DashTimers
{
    public float timer;
    public bool completed;
}

[Serializable]
public enum PlayerSystems
{
    AllParts, BotTop,
    BotBottom, Turning,
    Weapons, LeftWeapon,
    RightWeapon, Dashing
}

[Serializable]
public class PartPauseTracker
{

    public PlayerSystems playerSystem;

    public int pauseCount;

}