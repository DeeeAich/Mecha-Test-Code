using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{

    private bool firing = false;

    [SerializeField] Transform turnerObject;

    private PlayerBody myBody;

    private PlayerBody.WeaponInfo leftWeapon;
    private PlayerBody.WeaponInfo rightWeapon;

    public void LookDirection(Vector2 direction)
    {

        if (direction.magnitude > 0)
        {

            Quaternion lookDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));

            if (firing)
            {

            }

            turnerObject.rotation = lookDirection;

        }

    }

    public virtual void Start()
    {

        myBody = GetComponent<PlayerBody>();

        myBody.weaponLeft.LoadWeapon(leftWeapon);
        myBody.weaponRight.LoadWeapon(rightWeapon);

    }

}
