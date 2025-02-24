using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class HealOnKill : OnHealChip
{
    public float chanceToHeal = 15;
    public float percentageToHeal = 5;

    public override void TriggerAbility()
    {

        Health health = PlayerBody.PlayBody().GetComponent<Health>();
        health.TakeDamage(-health.maxHealth * 5 / 100);

    }


}
