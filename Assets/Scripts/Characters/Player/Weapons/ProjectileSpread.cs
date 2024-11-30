using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpread : ProjectileGun
{

    public int fireCount;
    public float spreadRange = 0.3f;
    public bool evenSpread;

    public override void LoadBullets()
    {
        for (int i = 0; i < maxAmmo * fireCount; i++)
        {
            GameObject genBullet = GameObject.Instantiate(projectile, projectileHolder);
            genBullet.name = "PlayerBullet";
        }
    }

    public override IEnumerator RepeatFire()
    {
        if (waitOnShot || !fireHeld || reloading)
            yield break;

        waitOnShot = true;

        float spreadDif = 2f / ((float)fireCount - 1f);

        for (int i = 0; i < fireCount; i++)
        {
            float bulDiv = 0;
            if (evenSpread)
                bulDiv = -1f + ((float)i * spreadDif);
            else
                bulDiv = Random.Range(-1.0f, 1.0f);

            GameObject newBullet = projectileHolder.GetChild(0).gameObject;
            newBullet.transform.parent = null;
            newBullet.transform.position = firePoint.position;
            newBullet.transform.rotation = firePoint.rotation;
            newBullet.transform.position += firePoint.right * bulDiv * spreadRange;
            newBullet.transform.rotation *= Quaternion.Euler(0, maxDiviation * bulDiv, 0);

        }
        myAnim.SetTrigger("Fire");

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
