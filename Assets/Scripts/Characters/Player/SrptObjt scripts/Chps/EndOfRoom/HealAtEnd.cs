using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "newRoomEndHeal", menuName = "Player/Chip/EndRoomChips/HealAtEnd")]
public class HealAtEnd : BEndChip
{

    public int healPercent = 15;

    public override void TriggerAbility()
    {
        Health health = PlayerBody.PlayBody().GetComponent<Health>();
        health.TakeDamage(-(health.maxHealth * healPercent )/100);

    }

}
