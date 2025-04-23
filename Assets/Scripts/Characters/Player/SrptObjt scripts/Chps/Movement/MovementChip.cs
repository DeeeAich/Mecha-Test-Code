using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementChip : Chip
{

    public enum MovementType
    {
        LegStat, DashStat,
        Trigger
    }

    public MovementType moveType;

}
