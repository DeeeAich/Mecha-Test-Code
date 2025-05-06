using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class MissileMotion : MoveProjectile
{
    float speed;
    [SerializeField] float timeto180 = 5f;
    GameObject target;
    public GameObject missileModel;

    public bool isDestroyed;
    public float deleteTimer;

    public UnityEvent onHit;



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

        globalVelocity = new Vector3(globalVelocity.x, 0, globalVelocity.z);
        

        if (isDestroyed)
        {
            deleteTimer += Time.fixedDeltaTime;
        }
        if (deleteTimer >= 2)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyMissile()
    {
        missileModel.SetActive(false);
        isDestroyed = true;
        deleteTimer = 0;
        onHit.Invoke();
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
                    DestroyMissile();


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
            DestroyMissile();
            //transform.GetComponentInChildren<Animator>().SetTrigger("impact");

        }
    }
}

