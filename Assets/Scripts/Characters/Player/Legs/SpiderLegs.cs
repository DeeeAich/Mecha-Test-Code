using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SpiderLegs : PlayerLegs
{

    public override IEnumerator Dash(Vector2 stickAmount)
    {

        if (dashing || curLegs.dashCharges == 0)
            yield break;

        print(curLegs.dashCharges);

        dashDirection = new Vector2();

        if (stickAmount.magnitude != 0)
        {
            dashDirection = (stickAmount * 10).normalized;
        }
        else if (curSpeed.magnitude != 0)
        {
            dashDirection = curSpeed.normalized;
        }
        else
        {
            yield break;
        }


        myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(true);
        myBody.myUI.Dashed();

        curLegs.dashCharges--;
        dashing = true;

        curSpeed = dashDirection * (myBody.legStats.dashDistance / myBody.legStats.dashTime);

        MultipleLegIkMover mover = GetComponentInChildren<MultipleLegIkMover>();

        mover.enabled = false;

        GetComponent<Health>().canTakeDamage = !myBody.legStats.invincibleDuringDash;
        yield return new WaitForSeconds(myBody.legStats.dashTime);
        GetComponent<Health>().canTakeDamage = true;
        dashing = false;

        myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(false);

        mover.enabled = true;

        yield return new WaitForSeconds(myBody.legStats.dashRecharge);

        curLegs.dashCharges++;

        yield return null;

    }

}
