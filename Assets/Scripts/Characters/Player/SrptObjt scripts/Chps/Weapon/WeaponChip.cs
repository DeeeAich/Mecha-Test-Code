using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChip : Chip
{

    public enum WeaponSubType
    {
        Generic, Projectile,
        StatusEffect
    }

    public WeaponSubType supType;

}
