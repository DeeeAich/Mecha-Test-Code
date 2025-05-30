using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SpiderLegs : PlayerLegs
{

    public override void Movement(Vector2 stickAmount)
    {
        base.Movement(stickAmount);
    }

    public override void Dash()
    {

        MultipleLegIkMover mover = GetComponentInChildren<MultipleLegIkMover>();

        bool removeFirst = false;

        for (int i = 0; i < dashTimers.Count; i++)
        {

            dashTimers[i].timer += Time.deltaTime;
            if (dashTimers[i].timer < 1 / ((1 / curLegs.dashTime) * dashMods.dashTime))
            {
                myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(true);

                curSpeed = dashDirection * ((curLegs.dashDistance * dashMods.dashDistance) / (1 / ((1 / curLegs.dashTime) * dashMods.dashTime)));

                mover.enabled = false;
                GetComponent<Health>().canTakeDamage = !myBody.legStats.invincibleDuringDash;

            }
            else if (!dashTimers[i].completed)
            {
                dashTimers[i].completed = true;
                dashing = false;
                mover.enabled = true;
                GetComponent<Health>().canTakeDamage = true;
                myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(false);

            }


            if (dashTimers[i].timer >= curLegs.dashRecharge * dashMods.dashRecharge + 1 / ((1 / curLegs.dashTime) * dashMods.dashTime))
            {
                curLegs.dashCharges++;

                removeFirst = true;

            }

        }

        if (removeFirst)
            dashTimers.RemoveAt(0);
    }

}
