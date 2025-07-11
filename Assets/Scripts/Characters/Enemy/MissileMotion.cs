﻿using System.Collections;
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

    private float startingYValue;

    private void Awake()
    {
        speed = localVelocity.magnitude;
        startingYValue = transform.position.y;
    }

    private void OnEnable()
    {
        //Level Generator -> Current Room -> OnCompleteRoom
        LevelGenerator.instance.currentRoom.GetComponent<Room>().onCompleteRoom.AddListener(DestroyMissile);
    }

    private void OnDisable()
    {
        LevelGenerator.instance.currentRoom.GetComponent<Room>().onCompleteRoom.RemoveListener(DestroyMissile);
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

        transform.position = new Vector3(transform.position.x, startingYValue, transform.position.z);

        if (isDestroyed)
        {
            deleteTimer -= Time.fixedDeltaTime;
            if (deleteTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
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
  
    }

    public void DestroyMissile()
    {
        missileModel.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;
        globalVelocity = Vector3.zero;
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

