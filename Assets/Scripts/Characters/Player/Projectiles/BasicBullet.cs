using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : Projectile
{

    [SerializeField] float speed;
    public ProjectileGun myGun;

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
                    gameObject.SetActive(false);
                    transform.parent = myGun.projectileHolder;
                    transform.localPosition = new Vector3();
                    transform.GetComponentInChildren<Animator>().SetTrigger("impact");
                }
            }
        }
        else
        {

            gameObject.SetActive(false);
            transform.parent = myGun.projectileHolder;
            transform.localPosition = new Vector3();
            transform.GetComponentInChildren<Animator>().SetTrigger("impact");

        }
    }

    public void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

}
