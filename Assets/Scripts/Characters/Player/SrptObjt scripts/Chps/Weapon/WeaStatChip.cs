using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponStatChip", menuName = "Player/Chip/Weapon/StatsChip")]
public class WeaStatChip : WeaponChip
{
    [Tooltip("Most numbers are percentage")]
    public WeaponStats myStatChange;

}
