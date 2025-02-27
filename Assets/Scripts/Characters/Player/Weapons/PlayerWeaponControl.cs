using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponControl : MonoBehaviour, IWeaponModifiable
{

    private bool firing = false;

    public Transform turnerObject;

    private PlayerBody myBody;

    public WeaponPickup leftWInfo;
    public Weapon leftWeapon;
    public List<WeaponChip> leftMods;
    public WeaponPickup rightWInfo;
    public Weapon rightWeapon;
    public List<WeaponChip> rightMods;

    public void LookDirection(Vector2 direction, bool isGamepad)
    {

        if (direction.magnitude > 0.2f)
        {

            Quaternion lookDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y / (isGamepad ? 1 :
                Mathf.Sin(45 * Mathf.Deg2Rad))));

            
            
            turnerObject.rotation = lookDirection;

        }

    }

    public void PressLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            leftWeapon.FirePress();
    }

    public void LiftLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            leftWeapon.FireRelease();
    }

    public void ReloadLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            StartCoroutine(leftWeapon.Reload());
    }

    public void FireRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            rightWeapon.FirePress();
    }

    public void LiftRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            rightWeapon.FireRelease();
    }

    public void ReloadRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot)
            StartCoroutine(rightWeapon.Reload());
    }

    public virtual void Start()
    {
        myBody = GetComponent<PlayerBody>();
        rightWeapon.myController = this;
        leftWeapon.myController = this;
    }

    public void ApplyChip(WeaponChip newChip, bool left)
    {

        Weapon weapon = null;

        if (left)
        {
            leftMods.Add(newChip);
            weapon = leftWeapon;
        }
        else
        {
            rightMods.Add(newChip);
            weapon = rightWeapon;
        }

        switch (newChip.supType)
        {
            case (WeaponChip.WeaponSubType.Generic):
                weapon.AddStats((WeaStatChip)newChip);
                break;
            case (WeaponChip.WeaponSubType.StatusEffect):
                ApplyMods((WeaStaEftChip)newChip, left ? leftWeapon : rightWeapon);
                break;
        }

    }

    public void ApplyMods(WeaStaEftChip applyChip, Weapon target)
    {

        IModable modableObject = target.GetComponent<IModable>();
        foreach(StatusInfo info in applyChip.AddStatuses)
        {

            modableObject.AddMod(info);

        }

    }

    public void ReApplyChips(bool left)
    {

        List<WeaponChip> movedChips = left ? leftMods : rightMods;

        if (left)
        {
            leftMods = new();
            foreach (WeaponChip chip in movedChips)
                ApplyChip(chip, left);
        }
        else
        {
            rightMods = new();
            foreach (WeaponChip chip in movedChips)
                ApplyChip(chip, left);
        }

    }

}
