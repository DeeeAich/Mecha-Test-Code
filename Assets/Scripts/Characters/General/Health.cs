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

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;


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
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        
        onTakeDamage.Invoke();

        if (health <= 0)
        {
            TriggerDeath();
        }
    }

    public void TriggerDeath()
    {
        onDeath.Invoke();
        if(destroyOnDeath) Destroy(gameObject);
    }
}
