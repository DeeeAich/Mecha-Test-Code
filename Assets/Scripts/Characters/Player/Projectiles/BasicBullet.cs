using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : Projectile
{

    [SerializeField] float speed;


    public void FixedUpdate()
    {

        transform.position += transform.forward * speed * Time.deltaTime;

    }

}
