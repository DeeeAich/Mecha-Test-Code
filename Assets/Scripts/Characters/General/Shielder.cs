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
    [SerializeField] internal float breakTime = 2f;

    [SerializeField] internal UnityEvent breakEvent;
    [SerializeField] internal bool canShield = true;

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
        if (canShield && VFX.shieldToggle == false && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude < rangeInit)
        {
            //Debug.Log("shielding");
            ShieldOn();
        }
        else if(VFX.shieldToggle && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude > rangeMax || targetHealth == null || !targetHealth.isAlive || !canShield)
        {
            TargetDied();
        }
        //Debug.Log(string.Format("{0} {1} {2}", canShield, VFX.shieldedTarget, ))
    }

    public void Break()
    {
        breakEvent.Invoke();
        VFX.shieldedTarget = null;
        //Debug.Log("New Target Discarded");
        targetHealth.onDeath.RemoveListener(TargetDied);
        VFX.shieldToggle = false;
        if(sm!=null)
        sm.removeFlag = true;
        HealthBar hb = targetHealth.GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            hb.shieldModifiers.Remove(sm);
        }
        sm = null;
        DisableForTime(breakTime);
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
        targetHealth.onDeath.AddListener(TargetDied);
        VFX.shieldedTarget = targetHealth.meshesRef;
        //Debug.Log("New Target Set");
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
    internal void TargetDied()
    {
        StartCoroutine(DisableForTime(breakTime));
        Break();
    }

    IEnumerator DisableForTime(float time)
    {
        canShield = false;
        float timer = time;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
        }
        canShield = true;
    }
}
