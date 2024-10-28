using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpread : ProjectileGun
{

    public int fireCount;
    public float spreadRange = 0.3f;
    public bool evenSpread;

    public override IEnumerator RepeatFire()
    {
        if (waitOnShot || !fireHeld || reloading)
            yield break;

        waitOnShot = true;
        for (int i = 1; i <= fireCount; i++)
        {
            float bulDiv = 0;
            if (evenSpread)
            {
                bulDiv = i / fireCount;

                bulDiv *= spreadRange * bulDiv > 0.5f ? 1 : -1;
            }
            else
                bulDiv = Random.Range(-fireCount, fireCount);
            GameObject newBullet = GameObject.Instantiate(projectile, firePoint);
            newBullet.transform.parent = null;
            newBullet.transform.position += firePoint.right * bulDiv;
            newBullet.transform.rotation *= Quaternion.Euler(0, maxDiviation * (curDivation / spreadRange), 0);
        }
        myAnim.SetTrigger("fire");

        yield return new WaitForSeconds(fireRate);

        waitOnShot = false;

        curAmmo -= shotCost;

        if (curAmmo <= 0)
        {
            StartCoroutine(Reload());
            yield break;
        }



        if (fireHeld && fullAuto)
            StartCoroutine(RepeatFire());
        else if (!fireHeld && fullAuto)
            curDivation = 0;

        yield return null;

    }

}
