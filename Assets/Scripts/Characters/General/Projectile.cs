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

    [Tooltip("0 means infinite")]
    public int pierceCount;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {

            health.TakeDamage(damage);

            pierceCount--;

            if (pierceCount == 0)
                Destroy(gameObject);


        }
        else
        {
            Destroy(gameObject);
        }

        /*if (other.TryGetComponent(out Health health))
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
        }*/
    }
}
