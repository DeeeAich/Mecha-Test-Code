using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public List<string> damageableHealthEntityTypes;

    [Header("For passing through targets, set pierce count to 0 to infinitely pierce")]
    public bool piercing;
    public int pierceCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            if (damageableHealthEntityTypes.Contains(health.entityType))
            {
                health.TakeDamage(damage);
                if (!piercing)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (pierceCount > 0)
                    {
                        pierceCount--;
                        if (pierceCount <= 0)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}
