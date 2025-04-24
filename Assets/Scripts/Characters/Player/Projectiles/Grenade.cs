using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : BasicBullet
{

    public override void Start()
    {
        base.Start();
    }

    public override void FixedUpdate()
    {
        if (gameObject.activeInHierarchy && !animating && !paused)
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            timer += Time.deltaTime;
            if (timer >= resetTime)
            {
                animating = true;
                transform.parent = myGun.projectileHolder;
                transform.localPosition = new Vector3();
                pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
                animating = false;

            }
        }
    }
}
