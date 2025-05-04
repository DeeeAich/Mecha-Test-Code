using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTriggerChip", menuName = "Player/Chip/Weapon/Trigger/SharedReload")]
public class SharedReload : WeaponTriggerChip
{

    public float reloadPercentage = 5;

    public override void ChipTriggerSetter(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            if (!actionAddedLeft)
            {
                startActionLeft += () => TriggerActivate(weapon);

                actionAddedLeft = true;
            }

            PlayerBody.Instance().triggers.reloadLeft += startActionLeft;
            addedCountLeft++;

        }
        else
        {
            if(!actionAddedRight)
            {
                startActionRight += () => TriggerActivate(weapon);

                actionAddedRight = true;
            }

            PlayerBody.Instance().triggers.reloadRight += startActionRight;
            addedCountRight++;

        }

    }

    public override void TriggerActivate(Weapon weapon)
    {

        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {

            PlayerWeaponControl.instance.rightWeapon.ExternalReload(reloadPercentage, true);

        }
        else
        {
            
            PlayerWeaponControl.instance.leftWeapon.ExternalReload(reloadPercentage, true);

        }

    }

    public override void ChipTriggerUnsetter(Weapon weapon)
    {

        for (int i = 0; i < (weapon == PlayerWeaponControl.instance.leftWeapon ? addedCountLeft : addedCountRight); i++)
        {
            if (weapon == PlayerWeaponControl.instance.leftWeapon)
            {
                PlayerBody.Instance().triggers.reloadLeft -= startActionLeft;
            }
            else
            {
                PlayerBody.Instance().triggers.reloadRight -= startActionRight;
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
