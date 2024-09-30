using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int curAmmo;
    public int maxAmmo;
    public int shotCost;
    public bool fullAuto;
    public float fireRate;
    public float reloadTime;
    public bool reloading;
    public GameObject weapon;
    public Transform firePoint;
    public Animator myAnim;
    public bool fireHeld;

    public float turnLimit;

    public bool waitOnShot;


    public virtual void Start()
    {
        myAnim = weapon.GetComponent<Animator>();

        myAnim.SetFloat("FireRate", 1 / fireRate);
        myAnim.SetFloat("ReloadSpeed", 1 / reloadTime);
    }

    public virtual void FirePress()
    {



    }

    public virtual void FireRelease()
    {



    }

    public virtual IEnumerator Reload()
    {

        if (reloading || waitOnShot)
            yield break;

        reloading = true;

        myAnim.SetTrigger("reload");
        
        yield return new WaitForSeconds(reloadTime);

        curAmmo = maxAmmo;
        reloading = false;

        if (fireHeld && fullAuto)
            FirePress();

        yield return null;

    }
}
