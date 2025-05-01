using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "newRoomEndHeal", menuName = "Player/Chip/Body/Trigger/HealChip")]
public class HealChip : BodyTriggerChip
{
    public float chance = 0;
    public int healPercent = 15;

    public override void TriggerAbility(PlayerBody myBody)
    {

        if (chance == 0 || chance > UnityEngine.Random.Range(0, 100))
        {
            Health health = myBody.GetComponent<Health>();
            health.TakeDamage(-(health.maxHealth * healPercent) / 100);
        }
    }

}
