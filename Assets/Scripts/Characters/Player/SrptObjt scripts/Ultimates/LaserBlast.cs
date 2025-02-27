using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlast : Ultimate
{

    public float laserWidth;
    public float laserMaxLength;
    private float ultTime;
    public float turnSpeed;

    public LayerMask toHit;

    private bool firing = false;

    public override void ActivateUltimate()
    {

        PlayerUltyControl.instance.RunAnimation("Fire");
        PlayerUltyControl.instance.recharging = true;
        PlayerBody.PlayBody().weaponHolder.TurnSpeedEffected(true, turnSpeed);
        ultTime = 0;
        firing = true;

    }

    public override void UltUpdate()
    {

        if (castTime > ultTime)
        {

            LineRenderer line = PlayerUltyControl.instance.ultimateCaster.GetComponentInChildren<LineRenderer>();

            Transform firePoint = PlayerUltyControl.instance.ultimateCaster.transform.GetChild(0);

            line.SetPosition(0, firePoint.position);

            RaycastHit hit;
            Vector3 secondPoint = new();
            RaycastHit[] damageTargets;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, laserMaxLength, toHit))
            {
                secondPoint = hit.point;

                damageTargets = Physics.SphereCastAll(firePoint.position, laserWidth, firePoint.forward, Vector3.Distance(hit.point, firePoint.position));

            }
            else
            {
                secondPoint = firePoint.position + (firePoint.forward * laserMaxLength);

                damageTargets = Physics.SphereCastAll(firePoint.position, laserWidth, firePoint.forward, laserMaxLength);
            }

            line.SetPosition(1, secondPoint);

            foreach (RaycastHit target in damageTargets)
                if (target.collider.TryGetComponent<Health>(out Health enemy))
                    enemy.TakeDamage(damages[0] * Time.deltaTime);

            ultTime += Time.deltaTime;

        }
        else if (firing)
        {

            firing = false;

            StartCoroutine(RechargeReset());

            PlayerBody.PlayBody().weaponHolder.TurnSpeedEffected(false, 0);
            PlayerUltyControl.instance.RunAnimation("Reset");

        }

    }

    public IEnumerator RechargeReset()
    {

        yield return new WaitForSeconds(rechargeTime);

        PlayerUltyControl.instance.recharging = true;

        PlayerUltyControl.instance.RunAnimation("Reload");

        yield return null;

    }

}
