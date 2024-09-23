using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    private PlayerBody myBody;
    private static Vector2 curSpeed;
    private GameObject myLegs;
    private bool dashing = false;
    private Vector3 dashPoint;

    public virtual void Movement(Vector2 stickAmount)
    {

        if (dashing)
            return;

        if (stickAmount.magnitude != 0)
        {
            curSpeed += stickAmount * myBody.curLegs.accelleration * Time.deltaTime;

            if (curSpeed.magnitude > (stickAmount * myBody.curLegs.speed).magnitude)
                curSpeed = stickAmount * myBody.curLegs.speed;
        }
        else
        {
            curSpeed -= curSpeed.normalized * Time.deltaTime * myBody.curLegs.accelleration;
            if (curSpeed.magnitude <= 0.5f)
                curSpeed = new Vector2();
        }

        transform.position += new Vector3(curSpeed.x, 0, curSpeed.y) * Time.deltaTime;
    }

    public virtual IEnumerator Dash(Vector2 stickAmount)
    {
        if (dashing || myBody.curLegs.dashCharges == 0)
            yield break;

        Vector2 direction = new Vector2();

        if(stickAmount.magnitude != 0)
        {
            direction = (stickAmount * 10).normalized;
        }
        else if(curSpeed.magnitude != 0)
        {
            direction = curSpeed.normalized;
        }
        else
        {
            yield break;
        }

        myBody.curLegs.dashCharges--;
        dashing = true;

        dashPoint = transform.position + new Vector3(direction.x, 0, direction.y) * myBody.curLegs.dashDistance;

        yield return new WaitUntil(DashHappening);

        transform.position = dashPoint;

        dashing = false;

        yield return new WaitForSeconds(myBody.curLegs.dashRecharge);

        myBody.curLegs.dashCharges++;

        yield return null;

    }

    public virtual bool DashHappening()
    {

        Vector3 movement = (dashPoint - transform.position) * myBody.curLegs.dashDistance / myBody.curLegs.dashTime;

        transform.position += movement * Time.deltaTime;

        curSpeed = new Vector2(movement.x, movement.z);

        return 0.3f > Vector3.Distance(dashPoint, transform.position);

    }

    private void Start()
    {
        myBody = GetComponent<PlayerBody>();
    }
}
