using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using Unity.VisualScripting;

public class BasicBullet : Projectile
{

    [SerializeField] internal float speed;
    [SerializeField] internal float resetTime = 2f;
    [SerializeField] internal float animationTime = 1.2f;
    internal bool animating = false;
    public ProjectileGun myGun;
    public EventReference hitMarkerSound;
    public bool hasHitMarkerSound =false;
    public bool paused = false;

    internal Critical critRoller;
    internal int pierceCounter = 0;
    internal float timer;

    public virtual void Start()
    {
        myGun.TryGetComponent<Critical>(out critRoller);

        pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;

    }

    public override void OnTriggerEnter(Collider other)
    {
        if (animating)
            return;

        if (other.TryGetComponent<Health>(out Health health))
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
                pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
                StopAllCoroutines();
                StartCoroutine(AnimationTimer());
            }
        }
        else
        {
            pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;

            StopAllCoroutines();
            StartCoroutine(AnimationTimer());

        }
    }

    public virtual void FixedUpdate()
    {
        if (gameObject.activeInHierarchy && !animating && !paused)
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            timer += Time.deltaTime;
            if(timer >= resetTime)
            {

                gameObject.SetActive(false);
                transform.parent = myGun.projectileHolder;
                transform.localPosition = new Vector3();
                pierceCounter = myGun.pierceCount + myGun.modifiers.piercing;
                animating = false;

            }
        }
    }

    public virtual IEnumerator AnimationTimer()
    {

        Animator bulletAnimator = GetComponentInChildren<Animator>();
        if (bulletAnimator != null)
        {
            animating = true;

            transform.GetComponentInChildren<Animator>().SetTrigger("impact");

            yield return new WaitForSeconds(animationTime);

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
