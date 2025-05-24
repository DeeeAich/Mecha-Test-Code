using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementTriggerChip : MovementChip
{
    
    public ChipEnums.Trigger chipTrigger;
    internal UnityAction startAction, resetAction, constantAction;
    internal bool addedAction;
    internal int addedActionCount;

    public virtual void Trigger(PlayerLegs playerLegs)
    {



    }

    public virtual void ChipTriggerSetter(PlayerLegs playerLegs)
    {

        addedActionCount++;

        if (!addedAction)
            startAction += () => Trigger(playerLegs);


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
            case (ChipEnums.Trigger.replenish):
                PlayerBody.Instance().triggers.replenishLeft += startAction;
                PlayerBody.Instance().triggers.replenishRight += startAction;
                break;
            case (ChipEnums.Trigger.dashed):
                    PlayerBody.Instance().triggers.dashed += startAction;
                break;
        }

        addedAction = true;

    }

    public virtual void ChipTriggerUnsetter()
    {

        startAction = null;

        for (int i = 0; i < addedActionCount; i++)
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
                case (ChipEnums.Trigger.replenish):
                    PlayerBody.Instance().triggers.replenishLeft -= startAction;
                    PlayerBody.Instance().triggers.replenishRight -= startAction;
                    break;
                case (ChipEnums.Trigger.dashed):
                    PlayerBody.Instance().triggers.dashed -= startAction;
                    break;
            }

        startAction = null;

        addedActionCount = 0;

        addedAction = false;

    }

}
