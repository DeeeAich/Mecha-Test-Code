using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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

        if (myController.leftWeapon == this)
        {
            PlayerBody.Instance().triggers.reloadLeft?.Invoke();
            PlayerBody.Instance().triggers.replenishLeft?.Invoke();
        }
        else
        {
            PlayerBody.Instance().triggers.reloadRight?.Invoke();
            PlayerBody.Instance().triggers.replenishRight?.Invoke();

        }

        myAnim.SetTrigger("Reload");

        float reloadsPerSecond = (1 / reloadTime) * modifiers.reloadSpeed;

        yield return new WaitForSeconds(1 / reloadsPerSecond);

        curAmmo = Mathf.RoundToInt(maxAmmo * modifiers.ammoCount);
        reloading = false;

        if (fireHeld && fullAuto)
            FirePress();

        yield return null;

    }

    public virtual void ExternalReload(float amount, bool percentage = false)
    {

        int addedCount = (int)amount;

        if (percentage)
            addedCount = Mathf.RoundToInt(amount * maxAmmo / 100);

        print(addedCount);

        if (addedCount <= 0)
            addedCount = 1;

        if (curAmmo + addedCount > maxAmmo)
            addedCount = maxAmmo - curAmmo;

        if (addedCount > 0)
        {
            print("Replenished");
            if (weapon == myController.leftWeapon)
                PlayerBody.Instance().triggers.replenishLeft?.Invoke();
            else
                PlayerBody.Instance().triggers.replenishRight?.Invoke();
        }

        curAmmo += addedCount;

    }

    public virtual void AddStats(WeaStatChip addStats)
    {
        addStats.myStatChange.AddStats(modifiers);
        SetAnimation();

    }

    public void TempStatsAdd(WeaponStats addStats)
    {
        addStats.AddStats(modifiers);
        SetAnimation();
    }

    public void TempStatRemove(WeaponStats weaponStats)
    {
        weaponStats.RemoveStats(modifiers);
        SetAnimation();
    }

    public void TempTimedStatsAdd(WeaponStats addStats, float timer)
    {

        addStats.AddStats(modifiers);
        SetAnimation();
        StartCoroutine(TempStatsTimedRemove(addStats, timer));

    }

    public IEnumerator TempStatsTimedRemove(WeaponStats removeStats, float timer)
    {

        yield return new WaitForSeconds(timer);

        removeStats.RemoveStats(modifiers);
        SetAnimation();

        yield return null;

    }

    public virtual void AddMod(StatusInfo modInfo)
    {
        foreach (ProjectileMod mod in GetComponents<ProjectileMod>())
            mod.AddModifiers(modInfo);
    }

    public virtual void SetAnimation()
    {
        float shotsPerSecond = (1 / fireRate) * modifiers.attackSpeed;
        float reloadsPerSecond = (1 / reloadTime) * modifiers.reloadSpeed;
        myAnim.SetFloat("FireRate", shotsPerSecond);
        myAnim.SetFloat("ReloadSpeed", reloadsPerSecond);
    }

}
