using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shielder : MonoBehaviour
{
    [SerializeField] ShieldVFXLineRendererManager VFX;
    [SerializeField] internal Health targetHealth;
    [SerializeField] internal float rangeMax = 7.5f, rangeInit = 5f;

    [SerializeField] internal UnityEvent breakEvent;
    // Start is called before the first frame update
    void Start()
    {
        VFX = GetComponentInChildren<ShieldVFXLineRendererManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (VFX.shieldToggle = false && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude < rangeInit)
        {
            VFX.shieldToggle = true;
        }
        else if(VFX.shieldToggle && (VFX.shieldedTarget.transform.position - gameObject.transform.position).magnitude > rangeMax)
        {
            Break();
        }
    }

    void Break()
    {
        breakEvent.Invoke();
    }

    public void Stop()
    {
        VFX.shieldToggle = false;
        enabled = false;
    }

    internal void SetTarget(GameObject target)
    {
        VFX.shieldedTarget = target;
    }

    /*
     * generate a class "HealthModifier" Shields is one
     * Health passes health to modifiers & modifiers pass back
     */

}
