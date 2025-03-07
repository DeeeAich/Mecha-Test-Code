using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealOnKillChip", menuName = "Player/Chips/OnKill/HealOnKill")]
public class HealOnKill : OnHealChip
{
    public float chanceToHeal = 15;
    public float percentageToHeal = 5;

    public override void TriggerAbility()
    {

        Health health = PlayerBody.Instance().GetComponent<Health>();
        health.TakeDamage(-health.maxHealth * 5 / 100);

    }


}
