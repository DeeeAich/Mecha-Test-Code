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
    private int leftSet = 0;
    private int rightSet = 0;

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

            PlayerBody.Instance().triggers.replenishLeft += startActionLeft;
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

            PlayerBody.Instance().triggers.replenishRight += startActionRight;
            actionAddedRight = true;

        }

    }

    public override void TriggerActivate(Weapon weapon)
    {
        if(weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            if (leftSet >= addedCountLeft)
                return;

            leftSet++;

            PlayerBody.Instance().triggers.fireLeft += resetActionLeft;

        }
        else
        {
            if (rightSet >= addedCountRight)
                return;

            rightSet++;

            PlayerBody.Instance().triggers.fireRight += resetActionRight;

        }

        weapon.TempStatsAdd(setStats);

    }

    public override void TriggerDeactivate(Weapon weapon)
    {
        weapon.TempStatRemove(setStats);
        charged = false;
        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            leftSet--;
            PlayerBody.Instance().triggers.fireLeft -= resetActionLeft;
        }
        else
        {
            rightSet--;
            PlayerBody.Instance().triggers.fireRight -= resetActionRight;
        }
    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            for (int i = 0; i < addedCountLeft; i++)
            {
                PlayerBody.Instance().triggers.replenishLeft -= startActionLeft;
                if (charged)
                    PlayerBody.Instance().triggers.fireLeft -= resetActionLeft;
            }
            resetActionLeft = null;
            startActionLeft = null;
            actionAddedLeft = false;
            addedCountLeft = 0;
            leftSet = 0;
        }
        else
        {
            for (int i = 0; i < addedCountRight; i++)
            {
                PlayerBody.Instance().triggers.replenishRight -= startActionRight;
                if (charged)
                    PlayerBody.Instance().triggers.fireRight -= resetActionRight;
            }
            startActionRight = null;
            resetActionRight = null;
            actionAddedRight = false;
            addedCountRight = 0;
            rightSet = 0;

        }

    }

}
