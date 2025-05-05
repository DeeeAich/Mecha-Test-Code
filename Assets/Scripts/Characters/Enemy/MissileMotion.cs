using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MissileMotion : MoveProjectile
{
    float speed;
    [SerializeField] float timeto180 = 5f;
    GameObject target;
    private void Awake()
    {
        speed = localVelocity.magnitude;
    }

    internal override void FixedUpdate()
    {
        if(target == null)
        {
            target = PlayerBody.Instance().gameObject;
        }
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        Vector3 targetPos = target.transform.position;
        targetPos.y = transform.position.y;
        Quaternion targetRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        float rotPerUpdate = (Time.fixedDeltaTime / timeto180) * 180f;
        float totalAngle = Quaternion.Angle(targetRot, transform.rotation);
        float t = rotPerUpdate / totalAngle;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, t);
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
                    Destroy(gameObject);
                    //transform.GetComponentInChildren<Animator>().SetTrigger("impact");
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
            Destroy(gameObject);
            //transform.GetComponentInChildren<Animator>().SetTrigger("impact");

        }
    }
}

