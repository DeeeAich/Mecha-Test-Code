using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewStatusEffectChip", menuName = "Player/Chip/Weapon/Trigger/FirstShot")]
public class FirstShot : WeaponTriggerChip
{
    public WeaponStats setStats;
    private bool charged;

    public override void ChipTriggerSetter(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            addedCountLeft++;

            if (!actionAddedLeft)
            {
                Debug.Log("Added left");
                startActionLeft += () => TriggerActivate(weapon);
                resetActionLeft += () => TriggerDeactivate(weapon);
            }

            PlayerBody.Instance().triggers.reloadLeft += startActionLeft;
            actionAddedLeft = true;
        }
        else
        {
            addedCountRight++;

            if (!actionAddedRight)
            {
                Debug.Log("Added Right");
                startActionRight += () => TriggerActivate(weapon);
                resetActionRight += () => TriggerDeactivate(weapon);
            }

            PlayerBody.Instance().triggers.reloadRight += startActionRight;
            actionAddedRight = true;

        }

    }

    public override void TriggerActivate(Weapon weapon)
    {
        weapon.TempStatsAdd(setStats);
        charged = true;

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
            PlayerBody.Instance().triggers.fireLeft += resetActionLeft;
        else
            PlayerBody.Instance().triggers.fireRight += resetActionRight;
    }

    public override void TriggerDeactivate(Weapon weapon)
    {
        weapon.TempStatRemove(setStats);
        charged = false;
        if (weapon == PlayerWeaponControl.instance.leftWeapon)
            PlayerBody.Instance().triggers.fireLeft -= resetActionLeft;
        else
            PlayerBody.Instance().triggers.fireRight -= resetActionRight;
    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            for (int i = 0; i < addedCountLeft; i++)
            {
                PlayerBody.Instance().triggers.reloadLeft -= startActionLeft;
                if (charged)
                    PlayerBody.Instance().triggers.fireLeft -= resetActionLeft;
            }
            resetActionLeft = null;
            startActionLeft = null;
            actionAddedLeft = false;
            addedCountLeft = 0;
        }
        else
        {
            for (int i = 0; i < addedCountRight; i++)
            {
                PlayerBody.Instance().triggers.reloadRight -= startActionRight;
                if (charged)
                    PlayerBody.Instance().triggers.fireRight -= resetActionRight;
            }
            startActionRight = null;
            resetActionRight = null;
            actionAddedRight = false;
            addedCountRight = 0;

        }

    }

}
