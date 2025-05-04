using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerWeaponControl : MonoBehaviour, IWeaponModifiable
{

    private bool firing = false;
    private float turnSpeed;

    public Transform turnerObject;

    private PlayerBody myBody;

    public WeaponPickup leftWInfo;
    public Weapon leftWeapon;
    public List<WeaponChip> leftMods;
    public WeaponPickup rightWInfo;
    public Weapon rightWeapon;
    public List<WeaponChip> rightMods;

    public UnityEvent leftFire;
    public UnityEvent rightFire;

    public static PlayerWeaponControl instance;

    public void LookDirection(Vector2 direction, bool isGamepad, Vector3 playerPos)
    {

        if (direction.magnitude > 0.2f)
        {

            Quaternion lookDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y / (isGamepad ? 1 :
                Mathf.Sin(45 * Mathf.Deg2Rad))));


            if (firing)
                turnerObject.rotation = Quaternion.Lerp(turnerObject.rotation, lookDirection, turnSpeed * Time.deltaTime);
            else
                turnerObject.rotation = lookDirection;

        }

        if (isGamepad)
            PlayerUI.instance.SetCursorLocation(playerPos, turnerObject.eulerAngles.y);

    }

    public void PressLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && leftWeapon != null)
        {
            leftFire.Invoke();
            leftWeapon.FirePress();
        }
    }

    public void LiftLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && leftWeapon != null)
            leftWeapon.FireRelease();
    }

    public void ReloadLeft(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && leftWeapon != null)
            StartCoroutine(leftWeapon.Reload());
    }

    public void PressRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && rightWeapon != null)
        {
            rightFire.Invoke();
            rightWeapon.FirePress();
        }
    }

    public void LiftRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && rightWeapon != null)
            rightWeapon.FireRelease();
    }

    public void ReloadRight(InputAction.CallbackContext context)
    {
        if (myBody.canShoot && rightWeapon != null)
            StartCoroutine(rightWeapon.Reload());
    }

    public virtual void Start()
    {
        myBody = GetComponent<PlayerBody>();

        instance = this;

        if(rightWeapon != null)
            rightWeapon.myController = this;
        if (leftWeapon != null)
            leftWeapon.myController = this;
    }

    public void ApplyChip(WeaponChip newChip, bool left)
    {

        print("Applying chip " + newChip.name);

        Weapon weapon = null;

        if (left)
        {
            leftMods.Add(newChip);
            weapon = leftWeapon;
            if (PlayerManager.instance != null)
                PlayerManager.instance.leftWeaponChips.Add(newChip);
        }
        else
        {
            rightMods.Add(newChip);
            weapon = rightWeapon;
            if (PlayerManager.instance != null)
                PlayerManager.instance.rightWeaponChips.Add(newChip);
        }

        switch (newChip.supType)
        {
            case (WeaponChip.WeaponSubType.Generic):
                weapon.AddStats((WeaStatChip)newChip);
                break;
            case (WeaponChip.WeaponSubType.StatusEffect):
                ApplyMods((WeaStaEftChip)newChip, left ? leftWeapon : rightWeapon);
                break;
            case (WeaponChip.WeaponSubType.Trigger):
                ApplyTriggers((WeaponTriggerChip)newChip, left);
                break;
        }

    }

    public void ApplyMods(WeaStaEftChip applyChip, Weapon target)
    {

        IModable modableObject = target.GetComponent<IModable>();
        foreach (StatusInfo info in applyChip.AddStatuses)
        {

            modableObject.AddMod(info);

        }

    }

    public void ApplyTriggers(WeaponTriggerChip chip, bool left)
    {

        chip.ChipTriggerSetter(left ? leftWeapon : rightWeapon);

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

    public void TurnSpeedEffected(bool turnAffected, float turnSpeedSet)
    {

        firing = turnAffected;

        turnSpeed = turnSpeedSet;

    }
}