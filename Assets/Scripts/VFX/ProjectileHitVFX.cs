using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitVFX : MonoBehaviour
{
    public ParticleSystem hitVFX;
    public int hitVFXEmission;
    public bool isEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default") && other.CompareTag("Untagged"))
        {
            hitVFX.Emit(hitVFXEmission);
        }

        if (!isEnemy)
        {
            if (other.CompareTag("Enemy"))
            {
                hitVFX.Emit(hitVFXEmission);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                hitVFX.Emit(hitVFXEmission);
            }
        }
    }

    public void ManualHitVFXTtrigger()
    {
        hitVFX.Emit(hitVFXEmission);
    }
}
