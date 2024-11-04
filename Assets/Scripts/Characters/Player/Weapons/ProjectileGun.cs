using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Weapon
{

    public GameObject projectile;

    public float maxDiviation;
    public float curDivation;

    public float divPerShot;
    public override void FirePress()
    {
        fireHeld = true;
        StartCoroutine(RepeatFire());
    }
    public override void FireRelease()
    {
        fireHeld = false;
    }

    public virtual IEnumerator RepeatFire()
    {
        if (waitOnShot || !fireHeld || reloading)
            yield break;

        waitOnShot = true;

        GameObject newBullet = GameObject.Instantiate(projectile, firePoint);
        newBullet.transform.parent = null;
        newBullet.transform.rotation *= Quaternion.Euler(0, Random.Range(-curDivation, curDivation), 0);

        myAnim.SetTrigger("Fire");

        yield return new WaitForSeconds(fireRate);

        waitOnShot = false;

        curAmmo -= shotCost;

        if(curAmmo <= 0)
        {
            StartCoroutine(Reload());
            yield break;
        }

        curDivation += divPerShot;
        if (curDivation > maxDiviation)
            curDivation = maxDiviation;

        if (fireHeld && fullAuto)
            StartCoroutine(RepeatFire());
        else if (!fireHeld && fullAuto)
            curDivation = 0;

        yield return null;
    }

}
