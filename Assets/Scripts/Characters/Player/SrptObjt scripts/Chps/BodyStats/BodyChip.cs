using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[CreateAssetMenu(fileName = "NewBodyChip", menuName = "Player/Chip/BodyChip")]
public class BodyChip : Chip
{

    public enum BodyType
    {
        Stat, EndRoom,
        Trigger,
        Constant, OnKill,
        OnDamage, OnHeal
    }

    public BodyType bodyType;

    public virtual void ChangeStats(BodyChip addChip)
    {



    }


}
