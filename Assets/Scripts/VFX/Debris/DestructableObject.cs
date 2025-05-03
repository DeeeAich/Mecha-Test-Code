using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    private bool destroyed;
    public Transform pushPos;
    public TriggerDebrisExplosion debrisExplosion;

    private void Start()
    {
        if (debrisExplosion == null)
        {
            debrisExplosion = GetComponent<TriggerDebrisExplosion>();
        }
        if(pushPos != null) pushPos.localPosition = new Vector3(pushPos.localPosition.x + Random.Range(-1f, 1f), pushPos.localPosition.y + Random.Range(-1f, 1f), pushPos.localPosition.z + Random.Range(-1f, 1f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyed) 
            return;

        if (other.CompareTag("Debris"))
            return;

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {

            debrisExplosion.explosionTrigger = true;
            destroyed = true;
        }
    }
}
