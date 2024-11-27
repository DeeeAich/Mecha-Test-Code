using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHackable, IBurnable
{
    public float health;
    public float maxHealth;

    public bool isAlive = true;
    public bool canTakeDamage = true;

    //public string entityType;
    public EntityType entityType;
    [Tooltip("This marks the outer bounds of the entity, for the purposes of positioning and scaling anything attatched to them (ie: healthbars, shield circles etc)")]
    public Vector3 entityBounds = new Vector3(1, 1, 1);

    public bool editorTakeDamage = false;
    public float editorDamageAmount = 100f;

    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyTimer = 3f;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    public List<DamageMod> damageMods;

    public GameObject meshesRef;//The shield renderer thing needs thee target to be where the meshes are


    internal virtual void Update()
    {
        if (editorTakeDamage)
        {
            TakeDamage(editorDamageAmount);
            editorTakeDamage = false;
        }
    }

    internal virtual void Awake()
    {
        health = maxHealth;
        damageMods = new List<DamageMod>();
    }

    internal virtual void TakeDamage(float amount)
    {
        if (!canTakeDamage || !isAlive) return;

        for (int i = 0; i < damageMods.Count; i++)
        {
            if (damageMods[i].removeFlag)
            {
                damageMods.RemoveAt(i);
                i--;
            }
        }

        float remainingDamage = amount;
        foreach (DamageMod mod in damageMods)
        {
            remainingDamage = mod.Modification(remainingDamage);
        }

        health -= remainingDamage;

        onTakeDamage.Invoke();



        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            TriggerDeath();
        }
    }

    internal virtual void TriggerDeath()
    {
        isAlive = false;
        canTakeDamage = false;
        onDeath.Invoke();
        if (destroyOnDeath) Destroy(gameObject, destroyTimer);
    }


    #region Hackable Interface Implementation
    Coroutine hackCoroutine;
    Hack hack;
    float hackTimer;
    public virtual void Hack(float percentage, float chance, float duration)
    {
        if(chance >= UnityEngine.Random.Range(0,100))
        {
            if (hackCoroutine == null)
            {
                hack = new Hack(percentage);
                damageMods.Add(hack);
                hackCoroutine = StartCoroutine(HackDecay());
                hackTimer = duration;
            }
            else
            {
                hackTimer = Mathf.Max(hackTimer, duration);
                hack.percent = Mathf.Max(hack.percent, percentage);
            }
        }
    }

    public virtual IEnumerator HackDecay()
    {
        while(hackTimer > 0)
        {
            hackTimer -= Time.deltaTime;
            yield return null;
        }
        damageMods.Remove(hack);
    }
    #endregion
    #region Burnable Interface Implementation

    #endregion
    /*SHIELD MECHANICS
    
    Shields can stack
    Smallest health sheild takes damage first //not yet implemented, just take damage in order applied (should be smallest to largest unless shields of varying sizes happen
    Shield provider gets stunned
    Healths have shieldable stat and shielded stat
    
    */
}

public abstract class DamageMod
{
    public bool removeFlag = false;
    public abstract float Modification(float damage);
}

public class Hack : DamageMod
{
    internal float percent;
    public Hack(float percent)
    {
        this.percent = percent;
    }

    public override float Modification(float damage)
    {
        return damage * (1f+percent/100f);
    }
}

public class ShieldModifier : DamageMod
{
    public float shieldHealth;
    readonly Shielder source;
    public ShieldModifier(float shieldHealth, Shielder source)
    {
        this.shieldHealth = shieldHealth;
        this.source = source;
    }

    public override float Modification(float damage)
    {
        float returnDamage;
        if (damage >= shieldHealth)
        {
            returnDamage = damage - shieldHealth;
            shieldHealth = 0;
            source.Break();
            removeFlag = true;
        }
        else
        {
            returnDamage = 0;
            shieldHealth -= damage;
        }
        return returnDamage;
    }
}
