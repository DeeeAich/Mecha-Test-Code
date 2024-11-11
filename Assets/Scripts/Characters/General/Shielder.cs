using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shielder : MonoBehaviour
{
    [SerializeField] ShieldVFXLineRendererManager VFX;
    [SerializeField] internal Health targetHealth;
    [SerializeField] internal float rangeMax = 7.5f, rangeInit = 5f;
    [SerializeField] internal float shieldHealth = 100f;

    [SerializeField] internal UnityEvent breakEvent;

    ShieldModifier sm;
    // Start is called before the first frame update
    void Start()
    {
        VFX = GetComponentInChildren<ShieldVFXLineRendererManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (VFX.shieldedTarget == null)
            return;
        if (VFX.shieldToggle == false && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude < rangeInit)
        {
            ShieldOn();
        }
        else if(VFX.shieldToggle && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude > rangeMax || targetHealth == null || !targetHealth.isAlive)
        {
            Break();
        }
    }

    public void Break()
    {
        breakEvent.Invoke();
        VFX.shieldedTarget = null;
        VFX.shieldToggle = false;
        if(sm!=null)
        sm.removeFlag = true;
        HealthBar hb = targetHealth.GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            hb.shieldModifiers.Remove(sm);
        }
        sm = null;
    }

    public void Stop()
    {
        VFX.shieldToggle = false;
        if (targetHealth != null)
        {
            HealthBar hb = targetHealth.GetComponentInChildren<HealthBar>();
            if (hb != null)
            {
                hb.shieldModifiers.Remove(sm);
            }
            if (sm != null)
                sm.removeFlag = true;
        }
        sm = null;
        enabled = false;
    }

    internal void SetTarget(GameObject target)
    {
        targetHealth = target.GetComponentInChildren<Health>();
        VFX.shieldedTarget = targetHealth.meshesRef;
    }


    internal void ShieldOn()
    {
        VFX.shieldToggle = true;
        sm = new ShieldModifier(shieldHealth, this);
        targetHealth.damageMods.Add(sm);
        HealthBar hb = targetHealth.GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            hb.shieldModifiers.Add(sm);
        }
    }
    /*
     * generate a class "HealthModifier" Shields is one
     * Health passes health to modifiers & modifiers pass back
     */

}
