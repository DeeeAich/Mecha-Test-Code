using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SpiderLegs : PlayerLegs
{

    public override IEnumerator Dash(Vector2 stickAmount)
    {

        if (dashing || myBody.curLegs.dashCharges == 0)
            yield break;

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

        myBody.curLegs.dashCharges--;
        dashing = true;

        curSpeed = dashDirection * (myBody.curLegs.dashDistance / myBody.curLegs.dashTime);

        MultipleLegIkMover mover = GetComponentInChildren<MultipleLegIkMover>();

        mover.enabled = false;

        yield return new WaitForSeconds(myBody.curLegs.dashTime);
        dashing = false;

        myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(false);

        mover.enabled = true;

        yield return new WaitForSeconds(myBody.curLegs.dashRecharge);

        myBody.curLegs.dashCharges++;

        yield return null;

    }

}
