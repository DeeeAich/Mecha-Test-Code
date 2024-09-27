using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject
{

    public int maxAmmo;
    public int shotCost;
    public bool fullAuto;
    public float fireRate;
    public float reloadTime;
    public GameObject projectile;
    public float turnLimit;

    public void LoadWeapon(PlayerBody.WeaponInfo weaponSlot)
    {
        weaponSlot.maxAmmo = maxAmmo;
        weaponSlot.curAmmo = maxAmmo;
        weaponSlot.fullAuto = fullAuto;
        weaponSlot.fireRate = fireRate;
        weaponSlot.reloadTime = reloadTime;
    }

}
