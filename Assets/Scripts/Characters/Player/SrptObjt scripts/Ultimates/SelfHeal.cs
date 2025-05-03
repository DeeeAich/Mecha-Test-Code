<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHeal : Ultimate
{

    public override void ActivateUltimate()
    {

        Health myHealth = PlayerBody.Instance().GetComponent<Health>();

        myHealth.TakeDamage(-damages[0] * myHealth.maxHealth);

        PlayerUltyControl.instance.recharging = true;

        StartCoroutine(HealReset());

    }

    public IEnumerator HealReset()
    {


        yield return new WaitForSeconds(rechargeTime);

        PlayerUltyControl.instance.recharging = false;

        yield return null;

    }

}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfHeal : Ultimate
{

    public override void ActivateUltimate()
    {

        Health myHealth = PlayerBody.Instance().GetComponent<Health>();

        myHealth.TakeDamage(-damages[0] * myHealth.maxHealth);

        PlayerUltyControl.instance.recharging = true;

        StartCoroutine(HealReset());

    }

    public IEnumerator HealReset()
    {


        yield return new WaitForSeconds(rechargeTime);

        PlayerUltyControl.instance.recharging = false;

        yield return null;

    }

}
>>>>>>> Stashed changes
