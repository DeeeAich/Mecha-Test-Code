using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBodyChip", menuName = "Player/Chip/BodyChip")]
public class BodyChip : Chip
{

    [Header("Stat Increase")]
    float health, regen, armour, shield;
    bool percentage = false;

}
