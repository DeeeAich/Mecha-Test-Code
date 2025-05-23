using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.ProBuilder.MeshOperations;

public class ProjectileGun : Weapon
{

    public Transform projectileHolder;
    public GameObject projectile;

    public float maxDiviation;
    public float curDivation;

    public float divPerShot;

    public bool hasBurst;
    public float timeBetweenBurst;
    public int burstCount = 1;

    public override void Start()
    {
        base.Start();
        LoadBullets();
    }

    public virtual void LoadBullets()
    {
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject genBullet = GameObject.Instantiate(projectile, projectileHolder);
            genBullet.name = "PlayerBullet";
            genBullet.GetComponent<BasicBullet>().myGun = this;
            genBullet.SetActive(false);
        }
    }

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



        for (int i = 0; i < burstCount; i++)
        {

            myAnim.SetTrigger("Fire");
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
            newBullet.transform.rotation *= Quaternion.Euler(0, UnityEngine.Random.Range(-curDivation - ((1 - modifiers.diviation) * (40 - maxDiviation)),
                                                                                curDivation + ((1 - modifiers.diviation) * (40 - maxDiviation))) , 0);
            newBullet.SetActive(true);
            newBullet.GetComponent<BasicBullet>().pierceCount = pierceCount + modifiers.piercing;
            newBullet.GetComponent<BasicBullet>().damage = damage[0] * modifiers.damage;

            curDivation += divPerShot;
            if (curDivation > maxDiviation)
                curDivation = maxDiviation;

            if (hasBurst)
            {
                float burstsPerSecond = (1 / timeBetweenBurst) * modifiers.attackSpeed;
                yield return new WaitForSeconds(1 / burstsPerSecond);
            }

        }

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


    public override void SetAnimation()
    {
        base.SetAnimation();

        if (hasBurst)
            myAnim.SetFloat("FireRate", (1 / timeBetweenBurst) * modifiers.attackSpeed);

    }
}
