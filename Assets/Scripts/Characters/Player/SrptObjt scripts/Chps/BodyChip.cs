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

[Serializable]
public class BodyStats
{
    public float health, armour, shield, shieldRegen;

    public void AddStats(BodyStats stats)
    {

        stats.health += health;
        stats.shieldRegen += shieldRegen;
        stats.armour += armour;
        stats.shield += shield;

    }
}
