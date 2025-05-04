using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;


public class WeaponTriggerChip : WeaponChip
{

    public ChipEnums.Trigger chipTrigger;
    public bool timed;

    internal UnityAction startActionLeft;
    internal UnityAction startActionRight;
    internal UnityAction constantActionLeft;
    internal UnityAction constantActionRight;
    internal UnityAction resetActionLeft;
    internal UnityAction resetActionRight;

    internal bool actionAddedLeft, actionAddedRight;

    internal int addedCountLeft = 0;
    internal int addedCountRight = 0;
    public virtual void TriggerActivate(Weapon weapon)
    {
        
    }

    public virtual void TriggerDeactivate(Weapon weapon)
    {
        
    }

    public virtual void TriggeredUpdate(Weapon weapon)
    {
        
    }

    public virtual void ChipTriggerSetter(Weapon weapon)
    {
        if (PlayerWeaponControl.instance.leftWeapon == weapon)
        {
            startActionLeft += () => TriggerActivate(weapon);
            addedCountLeft++;
        }
        else
        {
            startActionRight += () => TriggerActivate(weapon);
            addedCountRight++;
        }
        if(!weapon == PlayerWeaponControl.instance.leftWeapon ? actionAddedLeft : actionAddedRight)
            switch (chipTrigger)
            {
                case (ChipEnums.Trigger.Damaged):
                        PlayerBody.Instance().triggers.damaged += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.OnHeal):
                        PlayerBody.Instance().triggers.healed += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.OnKill):
                        PlayerBody.Instance().triggers.killedLeft += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        PlayerBody.Instance().triggers.killedRight += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.OnRoomClear):
                        PlayerBody.Instance().triggers.roomClear += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.moveStart):
                        PlayerBody.Instance().triggers.moveStart += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.moveEnd):
                        PlayerBody.Instance().triggers.moveEnd += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.OnShot):
                        PlayerBody.Instance().triggers.fireLeft += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;

                        PlayerBody.Instance().triggers.fireRight += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.reload):
                        PlayerBody.Instance().triggers.reloadLeft += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;

                        PlayerBody.Instance().triggers.reloadRight += PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                    break;
                case (ChipEnums.Trigger.constant):
                        PlayerBody.Instance().triggers.constant += PlayerWeaponControl.instance.leftWeapon == weapon ? constantActionLeft : constantActionRight;
                    break;
            }

        if (PlayerWeaponControl.instance.leftWeapon == weapon)
            actionAddedLeft = true;
        else
            actionAddedRight = true;
    }

    public virtual void ChipTriggerUnsetter(Weapon weapon)
    {

        for (int i = 0; i < (weapon == PlayerWeaponControl.instance.leftWeapon ? addedCountLeft : addedCountRight); i++)
            if(weapon  == PlayerWeaponControl.instance.leftWeapon ? actionAddedLeft : actionAddedRight)
                switch (chipTrigger)
                {
                    case (ChipEnums.Trigger.Damaged):
                            PlayerBody.Instance().triggers.damaged -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.OnHeal):
                            PlayerBody.Instance().triggers.healed -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.OnKill):
                            PlayerBody.Instance().triggers.killedLeft -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                            PlayerBody.Instance().triggers.killedRight -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.OnRoomClear):
                            PlayerBody.Instance().triggers.roomClear -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.moveStart):
                            PlayerBody.Instance().triggers.moveStart -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.moveEnd):
                            PlayerBody.Instance().triggers.moveEnd -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.OnShot):
                            PlayerBody.Instance().triggers.fireLeft -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;

                            PlayerBody.Instance().triggers.fireRight -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                    case (ChipEnums.Trigger.reload):
                            PlayerBody.Instance().triggers.reloadLeft -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;

                            PlayerBody.Instance().triggers.reloadRight -= PlayerWeaponControl.instance.leftWeapon == weapon ? startActionLeft : startActionRight;
                        break;
                }
        if (weapon == PlayerWeaponControl.instance.leftWeapon)
        {
            startActionLeft = null;
            resetActionLeft = null;
            constantActionLeft = null;
            actionAddedLeft = false;
            addedCountLeft = 0;
        }
        else
        {
            startActionLeft = null;
            resetActionLeft = null;
            constantActionLeft = null;
            actionAddedRight = false;
            addedCountRight = 0;
        }
    }
}
