using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileSpread : ProjectileGun
{

    public int fireCount;
    public float spreadRange = 0.3f;
    public bool evenSpread;

    private Transform gizmoFirepoint;

    public override void LoadBullets()
    {
        for (int i = 0; i < maxAmmo * fireCount; i++)
        {
            GameObject genBullet = GameObject.Instantiate(projectile, projectileHolder);
            genBullet.name = "PlayerBullet";
            genBullet.GetComponent<BasicBullet>().myGun = this;
            genBullet.SetActive(false);
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

            GameObject newBullet;
            if (projectileHolder.childCount == 0)
            {
                newBullet = GameObject.Instantiate(projectile, projectileHolder);
                newBullet.name = "PlayerBullet";
                newBullet.GetComponent<BasicBullet>().myGun = this;
                newBullet.SetActive(false);
            }
            else
                newBullet = projectileHolder.GetChild(0).gameObject;
            newBullet.transform.parent = null;
            newBullet.transform.position = firePoint.position;
            newBullet.transform.rotation = firePoint.rotation;
            newBullet.transform.position += firePoint.right * bulDiv * spreadRange;
            newBullet.transform.rotation *= Quaternion.Euler(0, bulDiv * (maxDiviation + ((1 - modifiers.diviation) * (40 - maxDiviation))), 0);
            newBullet.SetActive(true);
            newBullet.GetComponent<BasicBullet>().damage = damage[0] * modifiers.damage;
            newBullet.GetComponent<BasicBullet>().pierceCount = pierceCount + modifiers.piercing;

        }
        myAnim.SetTrigger("Fire");

        curAmmo -= shotCost;

        if (myController.leftWeapon == this)
            PlayerBody.Instance().triggers.fireLeft?.Invoke();
        else
            PlayerBody.Instance().triggers.fireRight?.Invoke();

        float shotsPerSecond = (1 / fireRate) * modifiers.attackSpeed;
        yield return new WaitForSeconds(1 / shotsPerSecond);

        waitOnShot = false;


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

    private void OnDrawGizmosSelected()
    {
        if (gizmoFirepoint == null)
        {
            gizmoFirepoint = new GameObject().transform;
            gizmoFirepoint.gameObject.name = "Gizmo Firepoint";
            gizmoFirepoint.parent = firePoint.transform;
            gizmoFirepoint.position = firePoint.position;
        }
        
        Gizmos.color = Color.yellow;
        
        float spreadDif = 2f / ((float) fireCount - 1f);

        for (int i = 0; i < fireCount; i++)
        {
            float bulDiv = 0;
            if (evenSpread)
                bulDiv = -1f + ((float)i * spreadDif);
            else
                bulDiv = Random.Range(-1.0f, 1.0f);

            gizmoFirepoint.transform.position = firePoint.position;
            gizmoFirepoint.transform.rotation = firePoint.rotation;
            gizmoFirepoint.transform.position += firePoint.right * bulDiv * spreadRange;
            gizmoFirepoint.rotation *= Quaternion.Euler(0, bulDiv * (maxDiviation + ((1 - modifiers.diviation) * (40 - maxDiviation))), 0);
            
            Gizmos.DrawLine(gizmoFirepoint.position, gizmoFirepoint.position + gizmoFirepoint.forward);
        }
    }
}
