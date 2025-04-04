using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using Unity.VisualScripting;

public class BasicBullet : Projectile
{

    [SerializeField] float speed;
    [SerializeField] float resetTime = 2f;
    bool animating = false;
    public ProjectileGun myGun;
    public EventReference hitMarkerSound;
    public bool hasHitMarkerSound =false;

    private Critical critRoller;
    private int pierceCounter = 0;

    public virtual void Start()
    {
        myGun.TryGetComponent<Critical>(out critRoller);

        pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;

    }

    public override void OnTriggerEnter(Collider other)
    {
        if (animating)
            return;

        if (other.TryGetComponent(out Health health))
        {
            if (hasHitMarkerSound)
            {
                AudioManager.instance.PlayOneShotSFX(hitMarkerSound, transform.position);
            }

            float modifiedDamage = critRoller.AdditiveDamage(damage);

            health.TakeDamage(modifiedDamage,out bool discard, modifiedDamage != damage);

            foreach (ProjectileMod modi in myGun.GetComponents<ProjectileMod>())
                modi.AttemptApply(other.gameObject);

            pierceCounter--;

            if (pierceCounter == -1)
            {
                StopAllCoroutines();
                StartCoroutine(AnimationTimer());
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(AnimationTimer());

        }
    }

    public virtual void FixedUpdate()
    {
        if (gameObject.activeInHierarchy && !animating)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    public IEnumerator AutoReset()
    {

        yield return new WaitForSeconds(resetTime);

        gameObject.SetActive(false);
        transform.parent = myGun.projectileHolder;
        transform.localPosition = new Vector3();
        pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
        animating = false;

        yield return null;
    }

    public IEnumerator AnimationTimer()
    {

        Animator bulletAnimator = GetComponentInChildren<Animator>();
        if (bulletAnimator != null)
        {
            animating = true;

            transform.GetComponentInChildren<Animator>().SetTrigger("impact");

            yield return new WaitForSeconds(1.2f);

            transform.GetComponentInChildren<Animator>().SetTrigger("return");
        }
            gameObject.SetActive(false);
            transform.parent = myGun.projectileHolder;
            transform.localPosition = new Vector3();
            pierceCount = myGun.pierceCount + myGun.modifiers.piercing;
            animating = false;
        yield return null;
    }
}
