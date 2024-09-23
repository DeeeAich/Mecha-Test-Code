using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{

    private bool firing = false;

    [SerializeField] Transform turnerObject;

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

}
