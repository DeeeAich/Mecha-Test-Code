using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BodyTriggerChip : BodyChip
{

    public ChipEnums.Trigger chipTrigger;
    internal UnityAction startAction, resetAction, constantAction;

    internal bool addedAction = false;
    internal int addedActionCount = 0;

    public virtual void ChipTriggerSetter()
    {

        Debug.Log("Setting triggers");

        PlayerBody playerBody = PlayerBody.Instance();

        if (!addedAction)
        {
            startAction += () => TriggerAbility(playerBody);
            addedAction = true;
            Debug.Log("Adding action for " + name);

        }
        switch (chipTrigger)
        {
            case (ChipEnums.Trigger.Damaged):
                    PlayerBody.Instance().triggers.damaged += startAction;
            break;
            case (ChipEnums.Trigger.OnHeal):
                    PlayerBody.Instance().triggers.healed += startAction;
                break;
            case (ChipEnums.Trigger.OnKill):
                    PlayerBody.Instance().triggers.killedLeft += startAction;
                    PlayerBody.Instance().triggers.killedRight += startAction;
                break;
            case (ChipEnums.Trigger.OnRoomClear):
                    PlayerBody.Instance().triggers.roomClear += startAction;
                break;
            case (ChipEnums.Trigger.moveStart):
                    PlayerBody.Instance().triggers.moveStart += startAction;
                break;
            case (ChipEnums.Trigger.moveEnd):
                    PlayerBody.Instance().triggers.moveEnd += startAction;
                break;
            case (ChipEnums.Trigger.OnShot):
                    PlayerBody.Instance().triggers.fireLeft += startAction;
                    PlayerBody.Instance().triggers.fireRight += startAction;
                break;
            case (ChipEnums.Trigger.reload):
                    PlayerBody.Instance().triggers.reloadLeft += startAction;
                    PlayerBody.Instance().triggers.reloadRight += startAction;
                break;
        }


    }

    public virtual void TriggerAbility(PlayerBody myBody)
    {

    }

    public virtual void ChipTriggerUnsetter()
    {

        for (int i = 0; i < addedActionCount; i++)
        {
            switch (chipTrigger)
            {
                case (ChipEnums.Trigger.Damaged):
                    PlayerBody.Instance().triggers.damaged -= startAction;
                    break;
                case (ChipEnums.Trigger.OnHeal):
                    PlayerBody.Instance().triggers.healed -= startAction;
                    break;
                case (ChipEnums.Trigger.OnKill):
                    PlayerBody.Instance().triggers.killedLeft -= startAction;
                    PlayerBody.Instance().triggers.killedRight -= startAction;
                    break;
                case (ChipEnums.Trigger.OnRoomClear):
                    PlayerBody.Instance().triggers.roomClear -= startAction;
                    break;
                case (ChipEnums.Trigger.moveStart):
                    PlayerBody.Instance().triggers.moveStart -= startAction;
                    break;
                case (ChipEnums.Trigger.moveEnd):
                    PlayerBody.Instance().triggers.moveEnd -= startAction;
                    break;
                case (ChipEnums.Trigger.OnShot):
                    PlayerBody.Instance().triggers.fireLeft -= startAction;
                    PlayerBody.Instance().triggers.fireRight -= startAction;
                    break;
                case (ChipEnums.Trigger.reload):
                    PlayerBody.Instance().triggers.reloadLeft -= startAction;
                    PlayerBody.Instance().triggers.reloadRight -= startAction;
                    break;
            }
        }

        addedAction = false;
        startAction = null;
        resetAction = null;
        constantAction = null;
        addedActionCount = 0;

    }

}
