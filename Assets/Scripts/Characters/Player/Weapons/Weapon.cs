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

    public virtual void Start()
    {
        myAnim.SetFloat("FireRate", 1 / fireRate);
        myAnim.SetFloat("ReloadSpeed", 1 / reloadTime);
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

        if (reloading || waitOnShot)
            yield break;

        reloading = true;

        myAnim.SetTrigger("Reload");
        
        yield return new WaitForSeconds(reloadTime);

        curAmmo = maxAmmo;
        reloading = false;

        if (fireHeld && fullAuto)
            FirePress();

        yield return null;

    }

    public virtual void AddMod(StatusInfo ModInfo)
    {

    }
}
