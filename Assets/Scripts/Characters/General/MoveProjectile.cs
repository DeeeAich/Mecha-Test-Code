using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectile : Projectile
{
    public Vector3 localVelocity;
    [SerializeField] public Vector3 globalVelocity;
    [SerializeField] private float despawnTimer = 20f;
    private bool piercing;

    // Start is called before the first frame update
    void Start()
    {
        globalVelocity = transform.rotation * localVelocity;
    }

    // Update is called once per frame
    internal virtual void FixedUpdate()
    {
        transform.position += globalVelocity * Time.fixedDeltaTime;
        if (despawnTimer > 0)
        {
            despawnTimer -= Time.fixedDeltaTime;
            if (despawnTimer <= 0)
            {
                Destroy(gameObject, 1f);
                globalVelocity = Vector3.zero;
                transform.GetComponentInChildren<Animator>().SetTrigger("impact");
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            if (damageableEntities.Contains(health.entityType))
            {
                health.TakeDamage(damage);

                pierceCount--;

                if (pierceCount == 0)
                {
                    Destroy(gameObject, 1f);
                    globalVelocity = Vector3.zero;
                    transform.GetComponentInChildren<Animator>().SetTrigger("impact");
                }/*
                else
                {
                    if (pierceCount > 0)
                    {
                        pierceCount--;
                        if (pierceCount <= 0)
                        {
                            Destroy(gameObject, 1f);
                            globalVelocity = Vector3.zero;
                            transform.GetComponentInChildren<Animator>().SetTrigger("impact");
                        }
                    }
                }*/
            }
        }
        else
        {
            Destroy(gameObject, 1f);
            globalVelocity = Vector3.zero;
            transform.GetComponentInChildren<Animator>().SetTrigger("impact");

        }
    }
}
