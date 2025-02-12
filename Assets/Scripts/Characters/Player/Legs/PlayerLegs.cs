using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    public PlayerBody myBody;
    public PlayerStatus myStats;
    public static Vector2 curSpeed;
    public GameObject myLegs;
    public bool dashing = false;
    public Vector3 dashDirection;
    public Rigidbody ridBy;

    public virtual void Movement(Vector2 stickAmount)
    {
        
        if (stickAmount.magnitude != 0 && !dashing)
        {
            curSpeed += stickAmount * myStats.curLegStats.acceleration * Time.deltaTime;

            if (curSpeed.magnitude > (stickAmount * myStats.curLegStats.speed).magnitude)
                curSpeed = stickAmount * myStats.curLegStats.speed;
        }
        else if(!dashing)
        {
            curSpeed -= curSpeed.normalized * Time.deltaTime * myStats.curLegStats.acceleration;
            if (curSpeed.magnitude <= 0.5f)
                curSpeed = new Vector2();
        }

        ridBy.velocity = new Vector3(curSpeed.x, 0, curSpeed.y);
    }

    public virtual IEnumerator Dash(Vector2 stickAmount)
    {
        if (dashing || myStats.curLegStats.dashCharges == 0)
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

    private void Start()
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
