using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    PLAYER, ENEMY, ENVIRONMENT
}

public class Projectile : MonoBehaviour
{
    public float damage;
    public List<string> damageableHealthEntityTypes;
    public List<EntityType> damageableEntities;

    [Header("For passing through targets, set pierce count to 0 to infinitely pierce")]
    public bool piercing;
    public int pierceCount;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            if (damageableEntities.Contains(health.entityType))
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
