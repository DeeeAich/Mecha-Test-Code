using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewBodyChip", menuName = "Player/Chip/BodyChip")]
public class BodyChip : Chip
{

    [Header("Stat Increase")]
    public BodyStats statChange;
    public bool percentage = false;

}
