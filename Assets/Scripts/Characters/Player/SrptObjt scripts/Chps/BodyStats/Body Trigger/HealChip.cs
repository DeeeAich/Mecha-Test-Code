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

        float testRandom = UnityEngine.Random.Range(0, 100);

        if (chance == 0 || chance >= testRandom)
        {
            Health health = myBody.GetComponent<Health>();
            health.TakeDamage(-(health.maxHealth * healPercent) / 100);
        }
    }

    public override void ChipTriggerSetter()
    {
        base.ChipTriggerSetter();
    }

    public override void ChipTriggerUnsetter()
    {
        base.ChipTriggerUnsetter();
    }

}
