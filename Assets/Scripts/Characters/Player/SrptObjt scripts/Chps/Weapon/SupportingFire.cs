using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTriggerChip", menuName = "Player/Chip/Weapon/Trigger/SupportingFire")]
public class SupportingFire : WeaponTriggerChip
{

    public WeaponStats stats;
    public int maxTriggers;
    private int triggersCountedLeft;
    private int triggersCountedRight;

    public override void ChipTriggerSetter(Weapon weapon)
    {

        if(weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            if (!actionAddedLeft)
            {

                startActionLeft += () => TriggerActivate(weapon);
                resetActionLeft += () => TriggerDeactivate(weapon);
                actionAddedLeft = true;

            }

            PlayerBody.Instance().triggers.killedLeft += startActionLeft;
            addedCountLeft++;

        }
        else
        {
            if (!actionAddedRight)
            {
                startActionRight += () => TriggerActivate(weapon);
                resetActionRight += () => TriggerDeactivate(weapon);
                actionAddedRight = true;
            }

            PlayerBody.Instance().triggers.killedRight += startActionRight;
            addedCountRight++;
        }

    }

    public override void TriggerActivate(Weapon weapon)
    {

        if ((weapon == PlayerWeaponControl.instance.leftWeapon ? triggersCountedLeft : triggersCountedRight) >= maxTriggers)
            return;

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            PlayerWeaponControl.instance.rightWeapon.TempStatsAdd(stats);

            PlayerBody.Instance().triggers.fireRight += resetActionLeft;

            triggersCountedLeft++;

        }
        else
        {

            PlayerWeaponControl.instance.leftWeapon.TempStatsAdd(stats);

            PlayerBody.Instance().triggers.fireLeft += resetActionRight;

            triggersCountedRight++;

        }

    }

    public override void TriggerDeactivate(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            PlayerWeaponControl.instance.rightWeapon.TempStatRemove(stats);

            PlayerBody.Instance().triggers.fireRight -= resetActionLeft;

            triggersCountedLeft--;

        }
        else
        {

            PlayerWeaponControl.instance.leftWeapon.TempStatRemove(stats);

            PlayerBody.Instance().triggers.fireLeft -= resetActionRight;

            triggersCountedRight--;

        }

    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {


        for (int i = 0; i < (weapon == PlayerWeaponControl.instance.leftWeapon ? addedCountLeft : addedCountRight); i++)
        {
            if (weapon == PlayerWeaponControl.instance.leftWeapon)
            {
                PlayerBody.Instance().triggers.killedLeft -= startActionLeft;
                PlayerBody.Instance().triggers.fireRight -= resetActionLeft;
            }
            else
            {
                PlayerBody.Instance().triggers.killedRight -= startActionRight;
                PlayerBody.Instance().triggers.fireLeft -= resetActionRight;
            }
        }

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            startActionLeft = null;
            resetActionLeft = null;
            actionAddedLeft = false;
            addedCountLeft = 0;
            triggersCountedLeft = 0;
        }
        else
        {
            startActionRight = null;
            resetActionRight = null;
            actionAddedRight = false;
            addedCountRight = 0;
            triggersCountedRight = 0;
        }


    }

}
