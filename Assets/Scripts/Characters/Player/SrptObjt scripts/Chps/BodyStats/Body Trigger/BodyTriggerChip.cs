using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BodyTriggerChip : BodyChip
{

    public ChipEnums.Trigger chipTrigger;

    public virtual void ChipTriggerSetter()
    {

        switch (chipTrigger)
        {
            case (ChipEnums.Trigger.Damaged):
                PlayerBody.Instance().triggers.damaged.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
            break;
            case (ChipEnums.Trigger.OnHeal):
                PlayerBody.Instance().triggers.healed.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
            case (ChipEnums.Trigger.OnKill):
                PlayerBody.Instance().triggers.killedLeft.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                PlayerBody.Instance().triggers.killedRight.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
            case (ChipEnums.Trigger.OnRoomClear):
                PlayerBody.Instance().triggers.roomClear.AddListener(delegate { TriggerAbility(PlayerBody.Instance());});
                break;
            case (ChipEnums.Trigger.moveStart):
                PlayerBody.Instance().triggers.moveStart.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
            case (ChipEnums.Trigger.moveEnd):
                PlayerBody.Instance().triggers.moveEnd.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
            case (ChipEnums.Trigger.OnShot):
                PlayerBody.Instance().triggers.fireLeft.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                PlayerBody.Instance().triggers.fireRight.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
            case (ChipEnums.Trigger.reload):
                PlayerBody.Instance().triggers.reloadLeft.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                PlayerBody.Instance().triggers.reloadRight.AddListener(delegate { TriggerAbility(PlayerBody.Instance()); });
                break;
        }

    }

    public virtual void TriggerAbility(PlayerBody myBody)
    {

    }

}
