using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float timeToHideHealthbarOnFullHealth = 1.5f;
    [SerializeField] private float healthbarSizeDifferenceScalar = 0.5f;
    
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image fillBar;
    [SerializeField] private Image shieldBar;
    
    private Health health;
    private bool showHealthBar;
    [HideInInspector] public List<ShieldModifier> shieldModifiers;
    
    private float pastHealth;
    private float shieldHealth;
    
    private float healthBarHideTimer = 0;
    private float canvasOrigionalScale;

    private void Start()
    {
        health = GetComponentInParent<Health>();

        transform.localPosition = new Vector3(0, health.entityBounds.y + 0.5f, 0);
        canvasOrigionalScale = canvas.transform.localScale.x;
        canvas.transform.localScale = new Vector3(canvasOrigionalScale + ((health.maxHealth - 100) / 100) * canvasOrigionalScale * healthbarSizeDifferenceScalar, canvasOrigionalScale, canvasOrigionalScale);
        
        pastHealth = health.maxHealth;
        shieldModifiers = new List<ShieldModifier>();
        healthBarHideTimer = 0;
    }

    private void Update()
    {
        showHealthBar = ((health.health < health.maxHealth || healthBarHideTimer > 0) && health.isAlive);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        
        if (healthBarHideTimer == 0)
        {
            if (pastHealth < health.maxHealth && health.health == health.maxHealth)
            {
                healthBarHideTimer = timeToHideHealthbarOnFullHealth;
            }
        }
        
        fillBar.fillAmount = health.health / health.maxHealth;

        RenderHealthModifications();
        
        if (healthBarHideTimer > 0) healthBarHideTimer -= Time.fixedDeltaTime;
        pastHealth = health.health;
        canvas.SetActive(showHealthBar);
    }

    private void RenderHealthModifications()
    {
        if (shieldModifiers.Count > 0)
        {
            shieldHealth = 0;
            for (int i = 0; i < shieldModifiers.Count; i++)
            {
                shieldHealth += shieldModifiers[i].shieldHealth;

            }
            shieldBar.fillAmount = shieldHealth / health.maxHealth;
            shieldBar.gameObject.SetActive(true);
            showHealthBar = true;
        }
        else
        {
            shieldBar.gameObject.SetActive(false);
        }
    }
}