using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    private PlayerBody myBody;
    private static Vector2 curSpeed;
    private GameObject myLegs;
    private bool dashing = false;
    private Vector3 dashDirection;
    Rigidbody ridBy;

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

        ridBy.velocity = new Vector3(curSpeed.x, 0, curSpeed.y);
    }

    public virtual IEnumerator Dash(Vector2 stickAmount)
    {
        if (dashing || myBody.curLegs.dashCharges == 0)
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

        myBody.curLegs.dashCharges--;
        dashing = true;

        yield return new WaitUntil(Dashing);

        dashing = false;

        yield return new WaitForSeconds(myBody.curLegs.dashRecharge);

        myBody.curLegs.dashCharges++;

        yield return null;

    }

    private bool Dashing()
    {

        

        return dashing;
    }

    private void Start()
    {
        myBody = GetComponent<PlayerBody>();
        ridBy = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(dashing && collision.gameObject.layer == 0)
        {



        }

    }
}
