using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileGun : Weapon
{

    public Transform projectileHolder;
    public GameObject projectile;

    public float maxDiviation;
    public float curDivation;

    public float divPerShot;

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

        myController.ReApplyChips(this == myController.leftWeapon);
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

        GameObject newBullet = projectileHolder.GetChild(0).gameObject;
        newBullet.transform.parent = null;
        newBullet.transform.position = firePoint.position;
        newBullet.transform.rotation = firePoint.rotation;
        newBullet.transform.rotation *= Quaternion.Euler(0, UnityEngine.Random.Range(-curDivation, curDivation), 0);
        newBullet.SetActive(true);

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

    public override void AddMod(StatusInfo modInfo)
    {

        string modName = modInfo.statusType.ToString();
        if(projectileHolder.GetChild(0).GetComponent(modName) != null)
        {

            foreach (Transform bullet in projectileHolder)
                foreach (ProjectileMod mod in bullet.GetComponents<ProjectileMod>())
                    mod.AddModifiers(modInfo);

        }
        else
            foreach (Transform bullet in projectileHolder)
            {

                bullet.gameObject.AddComponent(Type.GetType(modName));
                foreach (ProjectileMod mod in bullet.GetComponents<ProjectileMod>())
                    mod.AddModifiers(modInfo);
                bullet.GetComponent<BasicBullet>().modList.Add(bullet.GetComponent<ProjectileMod>());
            }

    }

}
