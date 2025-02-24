using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IModable
{
    public int curAmmo;
    public int maxAmmo;
    public int shotCost;
    public bool fullAuto;
    public float fireRate;
    public float reloadTime;
    public bool reloading;
    public float[] damage;
    public int pierceCount;
    public GameObject weapon;
    public Transform firePoint;
    public Animator myAnim;
    public bool fireHeld;

    public float turnLimit;

    public bool waitOnShot;

    public PlayerWeaponControl myController;

    public WeaponStats modifiers;
    public WeaponStats tempMods;

    public virtual void Start()
    {
        SetAnimation();
    }

    public virtual void FirePress()
    {

        if(reloading)
            fireHeld = true;

    }

    public virtual void FireRelease()
    {

        if(reloading)
            fireHeld = false;

    }

    public virtual IEnumerator Reload()
    {

        if (reloading || waitOnShot || curAmmo == maxAmmo)
            yield break;

        reloading = true;

        myAnim.SetTrigger("Reload");
        
        yield return new WaitForSeconds(reloadTime * modifiers.reloadSpeed);

        curAmmo = Mathf.RoundToInt(maxAmmo * modifiers.ammoCount);
        reloading = false;

        if (fireHeld && fullAuto)
            FirePress();

        yield return null;

    }

    public virtual void AddStats(WeaStatChip addStats)
    {

        addStats.myStatChange.AddStats(modifiers);
        SetAnimation();

    }

    public virtual void TempStatsAdd(WeaponStats addStats)
    {

        addStats.AddStats(tempMods);

    }

    public virtual void TempStatsRemove(WeaponStats removeStats)
    {


    }


    public virtual void AddMod(StatusInfo ModInfo)
    {

    }

    public virtual void SetAnimation()
    {
        myAnim.SetFloat("FireRate", 1 / fireRate * modifiers.attackSpeed);
        myAnim.SetFloat("ReloadSpeed", 1 / reloadTime * modifiers.reloadSpeed);
    }

}
