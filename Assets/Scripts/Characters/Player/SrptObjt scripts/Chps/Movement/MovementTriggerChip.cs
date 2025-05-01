using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTriggerChip : MovementChip
{
    
    public ChipEnums.Trigger chipTrigger;

    public virtual void Trigger(PlayerLegs playerLegs)
    {



    }

    public virtual void ChipTriggerSetter(PlayerLegs playerLegs)
    {

        switch (chipTrigger)
        {
            case (ChipEnums.Trigger.Damaged):
                PlayerBody.Instance().triggers.damaged.AddListener(delegate { Trigger(playerLegs); });
                break;
            case (ChipEnums.Trigger.OnHeal):
                PlayerBody.Instance().triggers.healed.AddListener(delegate { Trigger(playerLegs); });
                break;
            case (ChipEnums.Trigger.OnKill):
                PlayerBody.Instance().triggers.killedLeft.AddListener(delegate { Trigger(playerLegs); });
                PlayerBody.Instance().triggers.killedRight.AddListener(delegate { Trigger(playerLegs); });
                break;
            case (ChipEnums.Trigger.OnRoomClear):
                PlayerBody.Instance().triggers.roomClear.AddListener(delegate { Trigger(playerLegs); });
                break;
            case (ChipEnums.Trigger.moveStart):
                PlayerBody.Instance().triggers.moveStart.AddListener(delegate {  Trigger(playerLegs);  });
                break;
            case (ChipEnums.Trigger.moveEnd):
                PlayerBody.Instance().triggers.moveEnd.AddListener(delegate {  Trigger(playerLegs);  });
                break;
            case (ChipEnums.Trigger.OnShot):
                PlayerBody.Instance().triggers.fireLeft.AddListener(delegate {  Trigger(playerLegs);  });
                PlayerBody.Instance().triggers.fireRight.AddListener(delegate {  Trigger(playerLegs);  });
                break;
            case (ChipEnums.Trigger.reload):
                PlayerBody.Instance().triggers.reloadLeft.AddListener(delegate {  Trigger(playerLegs);  });
                PlayerBody.Instance().triggers.reloadRight.AddListener(delegate {  Trigger(playerLegs);  });
                break;
        }

    }

}
