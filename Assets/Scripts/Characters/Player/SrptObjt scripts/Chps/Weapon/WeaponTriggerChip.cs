using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponTriggerChip : WeaponChip
{

    public ChipEnums.Trigger chipTrigger;

    public virtual void TriggerActivate(Weapon weapon)
    {

    }

    public virtual void ChipTriggerSetter(Weapon weapon)
    {
        switch (chipTrigger)
        {
            case (ChipEnums.Trigger.Damaged):
                PlayerBody.Instance().triggers.damaged.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnHeal):
                PlayerBody.Instance().triggers.healed.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnKill):
                PlayerBody.Instance().triggers.killedLeft.AddListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.killedRight.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnRoomClear):
                PlayerBody.Instance().triggers.roomClear.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.moveStart):
                PlayerBody.Instance().triggers.moveStart.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.moveEnd):
                PlayerBody.Instance().triggers.moveEnd.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnShot):
                PlayerBody.Instance().triggers.fireLeft.AddListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.fireRight.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.reload):
                PlayerBody.Instance().triggers.reloadLeft.AddListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.reloadRight.AddListener(delegate { ChipTriggerSetter(weapon); });
                break;
        }

    }

    public virtual void ChipTriggerUnsetter(Weapon weapon)
    {
        switch (chipTrigger)
        {
            case (ChipEnums.Trigger.Damaged):
                PlayerBody.Instance().triggers.damaged.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnHeal):
                PlayerBody.Instance().triggers.healed.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnKill):
                PlayerBody.Instance().triggers.killedLeft.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.killedRight.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnRoomClear):
                PlayerBody.Instance().triggers.roomClear.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.moveStart):
                PlayerBody.Instance().triggers.moveStart.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.moveEnd):
                PlayerBody.Instance().triggers.moveEnd.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.OnShot):
                PlayerBody.Instance().triggers.fireLeft.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.fireRight.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
            case (ChipEnums.Trigger.reload):
                PlayerBody.Instance().triggers.reloadLeft.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                PlayerBody.Instance().triggers.reloadRight.RemoveListener(delegate { ChipTriggerSetter(weapon); });
                break;
        }
    }
}
