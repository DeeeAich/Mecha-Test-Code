using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveRound : BasicBullet
{

    public override void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.TryGetComponent<Health>(out Health health))
        {

            health.TakeDamage(damage);

        }

    }

    public override void Start()
    {

    }

    public override void FixedUpdate()
    {

    }

}
