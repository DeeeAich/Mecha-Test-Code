using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float health;
    //public string entityType;
    public EntityType entityType;

    public bool editorTakeDamage = false;
    public float editorDamageAmount = 100f;

    [SerializeField] private float maxHealth;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyTimer = 3f;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    public List<DamageMod> damageMods;

    public GameObject meshesRef;//The shield renderer thing needs thee target to be where the meshes are

    private void Update()
    {
        if(editorTakeDamage)
        {
            TakeDamage(editorDamageAmount);
            editorTakeDamage = false;
        }
    }

    private void Awake()
    {
        health = maxHealth;
        damageMods = new List<DamageMod>();
    }

    public void TakeDamage(float amount)
    {
        float remainingDamage = amount;
        foreach(DamageMod mod in damageMods)
        {
            remainingDamage = mod.Modification(remainingDamage);
        }

        health -= remainingDamage;
        
        onTakeDamage.Invoke();

        for(int i = 0; i < damageMods.Count; i++)
        {
            if(damageMods[i].removeFlag)
            {
                damageMods.RemoveAt(i);
                i--;
            }
        }

        if (health <= 0)
        {
            TriggerDeath();
        }
    }

    public void TriggerDeath()
    {
        onDeath.Invoke();
        if(destroyOnDeath) Destroy(gameObject, destroyTimer);
    }
    
    ///SHIELD MECHANICS
    ///
    ///Shields can stack
    ///Smallest health sheild takes damage first //not yet implemented, just take damage in order applied (should be smallest to largest unless shields of varying sizes happen
    ///Shield provider gets stunned
    ///Healths have shieldable stat and shielded stat
    ///
    /// 
}

public abstract class DamageMod
{
    public bool removeFlag = false;
    public abstract float Modification(float damage);
}

public class ShieldModifier:DamageMod
{
    float shieldHealth;
    Shielder source;
    public ShieldModifier(float shieldHealth, Shielder source)
    {
        this.shieldHealth = shieldHealth;
        this.source = source;
    }

    public override float Modification(float damage)
    {
        float returnDamage = 0;
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
