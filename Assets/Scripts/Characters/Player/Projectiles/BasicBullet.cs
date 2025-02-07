using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : Projectile
{

    [SerializeField] float speed;
    [SerializeField] float resetTime = 2f;
    public ProjectileGun myGun;

    private Critical critRoller;
    private int pierceCounter = 0;

    private void Start()
    {
        critRoller = myGun.GetComponent<Critical>();
        pierceCounter = myGun.pierceCount;

    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            float modifiedDamage = critRoller.AdditiveDamage(damage);

            health.TakeDamage(modifiedDamage);

            foreach (ProjectileMod modi in myGun.GetComponents<ProjectileMod>())
                modi.AttemptApply(other.gameObject);

            pierceCounter--;

            if (pierceCounter == 0)
            {
                gameObject.SetActive(false);
                transform.parent = myGun.projectileHolder;
                transform.localPosition = new Vector3();
                transform.GetComponentInChildren<Animator>().SetTrigger("impact");
                StopCoroutine(AutoReset());
            }
        }
        else
        {

            gameObject.SetActive(false);
            transform.parent = myGun.projectileHolder;
            transform.localPosition = new Vector3();
            transform.GetComponentInChildren<Animator>().SetTrigger("impact");
            pierceCounter = myGun.pierceCount;
            StopCoroutine(AutoReset());

        }
    }

    public void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    public IEnumerator AutoReset()
    {

        yield return new WaitForSeconds(resetTime);

        gameObject.SetActive(false);
        transform.parent = myGun.projectileHolder;
        transform.localPosition = new Vector3();
        pierceCounter = myGun.pierceCount;

        yield return null;
    }

}
