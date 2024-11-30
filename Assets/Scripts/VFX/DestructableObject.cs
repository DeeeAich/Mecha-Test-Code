using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public bool hasHealth;
    public float health;
    private bool destroyed;
    public TriggerDebrisExplosion debrisExplosion;

    private void Start()
    {
        if (debrisExplosion == null)
        {
            debrisExplosion = GetComponent<TriggerDebrisExplosion>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyed) 
            return;


            debrisExplosion.breakDirection = other.transform.position;
            debrisExplosion.explosionTrigger = true;
            destroyed = true;

    }
}
