using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        StartCoroutine(AnimationTimer());


    }

    public override IEnumerator AnimationTimer()
    {
        animating = true;
        timer = 0;

        RaycastHit[] enemiesHit = Physics.SphereCastAll(transform.position + Vector3.down, explRadius, Vector3.up, 2, enemies);

        GetComponentInChildren<Animator>().SetTrigger("impact");

        yield return new WaitForSeconds(0.1f);

        float modifiedDamage = critRoller.AdditiveDamage(damage);

        print("Touched " + enemiesHit.Length);

        foreach(RaycastHit enemy in enemiesHit)
            if(enemy.transform.gameObject.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.TakeDamage(modifiedDamage, myGun == myGun.myController.leftWeapon ? "left" : "right", critRoller.lastCrit);

                foreach (ProjectileMod modi in myGun.GetComponents<ProjectileMod>())
                    modi.AttemptApply(enemy.transform.gameObject);
            }

        yield return new WaitForSeconds(animationTime);

        transform.parent = myGun.projectileHolder;
        transform.localPosition = new Vector3();
        pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
        animating = false;

        GetComponentInChildren<Animator>().SetTrigger("return");

        gameObject.SetActive(false);

        yield return null;

    }
}
