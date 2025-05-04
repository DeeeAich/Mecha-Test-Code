using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffectChip", menuName = "Player/Chip/Weapon/Trigger/RepeatFire")]
public class RepeatFire : WeaponTriggerChip
{
    public WeaponStats stats;

    public override void ChipTriggerSetter(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            if (!actionAddedLeft)
            {

                Debug.Log("Adding stats trigger");
                startActionLeft += () => TriggerActivate(weapon);
                resetActionLeft += () => TriggerDeactivate(weapon);
                actionAddedLeft = true;
            }

            PlayerBody.Instance().triggers.fireLeft += startActionLeft;

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

            PlayerBody.Instance().triggers.fireRight += startActionRight;

            addedCountRight++;

        }

    }

    public override void TriggerActivate(Weapon weapon)
    {

        weapon.TempStatsAdd(stats);

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
            PlayerBody.Instance().triggers.replenishLeft += resetActionLeft;
        else
            PlayerBody.Instance().triggers.replenishRight += resetActionRight;

    }

    public override void TriggerDeactivate(Weapon weapon)
    {

        weapon.TempStatRemove(stats);

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
            PlayerBody.Instance().triggers.replenishLeft -= resetActionLeft;
        else
            PlayerBody.Instance().triggers.replenishRight -= resetActionRight;

    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {

        for(int i = 0; i < (weapon == PlayerWeaponControl.instance.leftWeapon ? addedCountLeft : addedCountRight); i++)
        {
            if (weapon == PlayerWeaponControl.instance.leftWeapon)
            {
                PlayerBody.Instance().triggers.fireLeft -= startActionLeft;
                PlayerBody.Instance().triggers.replenishLeft -= resetActionLeft;
            }
            else
            {
                PlayerBody.Instance().triggers.fireRight -= startActionRight;
                PlayerBody.Instance().triggers.replenishRight -= resetActionRight;
            }
        }

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            startActionLeft = null;
            resetActionLeft = null;
            actionAddedLeft = false;
            addedCountLeft = 0;
        }
        else
        {
            startActionRight = null;
            resetActionRight = null;
            actionAddedRight = false;
            addedCountRight = 0;
        }


    }

}
