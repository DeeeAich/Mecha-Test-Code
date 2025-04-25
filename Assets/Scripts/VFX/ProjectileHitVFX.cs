using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitVFX : MonoBehaviour
{
    public ParticleSystem hitVFX;
    public int hitVFXEmission;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default") && other.CompareTag("Untagged"))
        {
            hitVFX.Emit(hitVFXEmission);
        }
        if (other.CompareTag("Enemy"))
        {
            hitVFX.Emit(hitVFXEmission);
        }
    }
}
