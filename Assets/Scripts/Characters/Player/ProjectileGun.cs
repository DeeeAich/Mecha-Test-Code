using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : Weapon
{

    public GameObject projectile;

    private bool waitOnShot;

    public override void FirePress()
    {
        fireHeld = true;
        StartCoroutine(RepeatFire());
    }
    public override void FireRelease()
    {
        fireHeld = false;
    }


    private IEnumerator RepeatFire()
    {
        if (waitOnShot || !fireHeld || reloading)
            yield break;

        waitOnShot = true;

        GameObject newBullet = GameObject.Instantiate(projectile, firePoint);
        newBullet.transform.parent = null;

        myAnim.SetTrigger("fire");

        yield return new WaitForSeconds(fireRate);

        waitOnShot = false;

        curAmmo -= shotCost;

        if(curAmmo <= 0)
        {
            StartCoroutine(Reload());
            yield break;
        }    

        if (fireHeld && fullAuto)
            StartCoroutine(RepeatFire());

        yield return null;
    }

}
