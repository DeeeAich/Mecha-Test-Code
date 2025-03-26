using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class EnemyHealth : Health, IShortCircuitable
{
    internal bool isShieldable = true;
    internal BehaviourTree brain;
    //Lazy Initialization (don't get component if never needed)
    CharacterVFXManager _characterVFX;
    CharacterVFXManager VFXManager
    {
        get
        {
            if(_characterVFX == null)
                _characterVFX = GetComponentInChildren<CharacterVFXManager>();
            return _characterVFX;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        meshesRef = GetComponentInChildren<MeshFilter>().gameObject;
        brain = GetComponent<BehaviourTree>();
    }


    public virtual void ShortCircuit(float chance, float time)
    {
        if (chance > UnityEngine.Random.Range(0f, 100f))
        {
            if (VFXManager != null)
            {
                VFXManager.ToggleEffectVFX(effect.ShortCircuit, true);
            }
            brain.StopForTime(time);
            //apply VFX for time
        }
    }

    internal override void TriggerDeath()
    {
        base.TriggerDeath();
        isShieldable = false;
        TriggerDebrisExplosion tde = GetComponentInChildren<TriggerDebrisExplosion>();
        if (tde != null)
        {
            tde.TriggerExplosion();
        }
        Collider[] c = GetComponents<Collider>();
        if (c.Length > 0)
        {
            foreach (Collider x in c)
            {
                x.enabled = false;
            }
        }
        gameObject.tag = "Untagged";
        EnemyGun[] guns = GetComponentsInChildren<EnemyGun>();
        foreach (EnemyGun g in guns)
        {
            g.BeGone();
        }
        foreach (Transform child in transform)
        {
            if (transform.parent != null && transform.parent.parent != null && !child.gameObject.TryGetComponent<HealthBar>(out HealthBar bar))
                child.parent = transform.parent.parent;
        }
        brain.Die();
        Destroy(gameObject);
    }

    

    internal override DamageEventInfo TakeDamage(float amount, string source, int critCount)
    {
        DamageEventInfo info = base.TakeDamage(amount, source, critCount);
        EnemyDamageNumberSpawner.instance.SpawnDamageNumber(info, transform.position);
        return info;
    }

    internal override float TakeDamage(float amount, out bool isShield, bool isCrit = false)
    {
        float moddedAmount = base.TakeDamage(amount, out bool discardShield, isCrit);
        EnemyDamageNumberSpawner.instance.SpawnDamageNumber(moddedAmount, transform.position, isCrit, discardShield);
        isShield = discardShield;
        return moddedAmount;
    }
    internal override float TakeDamage(float amount, bool isCrit = false)
    {
        float moddedAmount = base.TakeDamage(amount, out bool discardShield, isCrit);
        EnemyDamageNumberSpawner.instance.SpawnDamageNumber(moddedAmount, transform.position, isCrit, discardShield);
        return moddedAmount;
    }

}
