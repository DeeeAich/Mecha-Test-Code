using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponControl : MonoBehaviour, IWeaponModifiable
{

    private bool firing = false;

    public Transform turnerObject;

    private PlayerBody myBody;

    public Weapon leftWeapon;
    public Weapon rightWeapon;

    public void LookDirection(Vector2 direction, bool isGamepad)
    {

        if (direction.magnitude > 0.2f)
        {

            Quaternion lookDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y / (isGamepad ? 1 :
                Mathf.Sin(45 * Mathf.Deg2Rad))));

            if (firing)
            {

            }

            turnerObject.rotation = lookDirection;

        }

    }

    public void PressLeft(InputAction.CallbackContext context)
    {
        if(myBody.canShoot)
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
    }

    public void ApplyChip(WeaponChip newChip, bool left)
    {

        switch (newChip.supType)
        {
            case (WeaponChip.WeaponSubType.Generic):

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

}
