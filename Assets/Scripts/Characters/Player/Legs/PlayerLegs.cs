using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    public PlayerBody myBody;
    public PlayerBody.LegInfo curLegs;
    public static Vector2 curSpeed;
    public GameObject myLegs;
    public bool dashing = false;
    public Vector3 dashDirection;
    public Rigidbody ridBy;

    public virtual void Movement(Vector2 stickAmount)
    {

        if (stickAmount.magnitude != 0 && !dashing)
        {
            curSpeed += stickAmount * myBody.legStats.speed;

            if (curSpeed.magnitude > (stickAmount * myBody.legStats.speed).magnitude)
                curSpeed = stickAmount * myBody.legStats.speed;
        }
        else if (!dashing)
        {
            curSpeed = new Vector2();
        }

        ridBy.velocity = new Vector3(curSpeed.x, 0, curSpeed.y);
    }

    public virtual IEnumerator Dash(Vector2 stickAmount)
    {
        if (dashing || myBody.legStats.dashCharges == 0)
            yield break;

        dashDirection = new Vector2();

        if(stickAmount.magnitude != 0)
        {
            dashDirection = (stickAmount * 10).normalized;
        }
        else if(curSpeed.magnitude != 0)
        {
            dashDirection = curSpeed.normalized;
        }
        else
        {
            yield break;
        }

        myLegs.GetComponent<MultipleLegIkMover>().ToggleDashParticles(true);
        myBody.myUI.Dashed();

        yield return null;

    }

    public virtual void Start()
    {
        myBody = GetComponent<PlayerBody>();
        ridBy = GetComponent<Rigidbody>();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {

        if(dashing && collision.gameObject.layer == 0)
        {



        }

    }
}
