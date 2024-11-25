using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponControl : MonoBehaviour
{

    private bool firing = false;

    [SerializeField] Transform turnerObject;

    private PlayerBody myBody;

    public Weapon leftWeapon;
    public Weapon rightWeapon;

    public void LookDirection(Vector2 direction, bool isGamepad)
    {

        if (direction.magnitude > 0)
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
        leftWeapon.FirePress();
    }

    public void LiftLeft(InputAction.CallbackContext context)
    {
        leftWeapon.FireRelease();
    }

    public void ReloadLeft(InputAction.CallbackContext context)
    {
        StartCoroutine(leftWeapon.Reload());
    }

    public void FireRight(InputAction.CallbackContext context)
    {
        rightWeapon.FirePress();
    }

    public void LiftRight(InputAction.CallbackContext context)
    {
        rightWeapon.FireRelease();
    }

    public void ReloadRight(InputAction.CallbackContext context)
    {
        StartCoroutine(rightWeapon.Reload());
    }

    public virtual void Start()
    {

        myBody = GetComponent<PlayerBody>();
    }

}
