using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public bool hasHealth;
    public float health;
    private bool destroyed;
    public Transform pushPos;
    public TriggerDebrisExplosion debrisExplosion;

    private void Start()
    {
        if (debrisExplosion == null)
        {
            debrisExplosion = GetComponent<TriggerDebrisExplosion>();
        }
        pushPos.localPosition = new Vector3(pushPos.localPosition.x, pushPos.localPosition.y + Random.Range(-1f, 1f), pushPos.localPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyed) 
            return;

        if (other.CompareTag("Debris"))
            return;

            debrisExplosion.explosionTrigger = true;
            destroyed = true;

    }
}
