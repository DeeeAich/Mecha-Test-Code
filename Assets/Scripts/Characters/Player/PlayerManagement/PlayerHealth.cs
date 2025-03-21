using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    internal override float TakeDamage(float amount, bool isCrit = false)
    {

        if (amount < 0 && health != maxHealth)
            PlayerBody.Instance().TriggerOnHeal((int)amount);
        else if (amount > 0)
            PlayerBody.Instance().TriggerOnDamage();

        return base.TakeDamage(amount, isCrit);
    }

}
