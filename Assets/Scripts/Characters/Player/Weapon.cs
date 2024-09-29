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

    public virtual void Start()
    {
        myAnim = weapon.GetComponent<Animator>();
    }

    public virtual void FirePress()
    {



    }

    public virtual void FireRelease()
    {



    }

    public virtual IEnumerator Reload()
    {

        if (reloading)
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
