using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Grenade : BasicBullet
{

    [SerializeField] LayerMask enemies;
    [SerializeField] internal float explRadius = 2;

    public override void Start()
    {
        base.Start();
    }

    public override void FixedUpdate()
    {
        if (gameObject.activeInHierarchy && !animating && !paused)
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            timer += Time.deltaTime;
            if (timer >= resetTime)
            {

                StartCoroutine(AnimationTimer());

            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {

        if (animating)
            return;

        if (hasHitMarkerSound)
                AudioManager.instance.PlayOneShotSFX(hitMarkerSound, transform.position);



    }

    public override IEnumerator AnimationTimer()
    {
        animating = true;

        GetComponentInChildren<Animator>().SetTrigger("impact");

        yield return new WaitForSeconds(animationTime / 2);

        float modifiedDamage = critRoller.AdditiveDamage(damage);

        RaycastHit[] enemiesHit = Physics.SphereCastAll(transform.position, explRadius, new Vector3(), 0, enemies);

        foreach(RaycastHit enemy in enemiesHit)
            if(enemy.transform.gameObject.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(modifiedDamage);
            }

        yield return new WaitForSeconds(animationTime / 2);

        transform.parent = myGun.projectileHolder;
        transform.localPosition = new Vector3();
        pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
        animating = false;

        GetComponentInChildren<Animator>().SetTrigger("return");

        yield return null;

    }
}
