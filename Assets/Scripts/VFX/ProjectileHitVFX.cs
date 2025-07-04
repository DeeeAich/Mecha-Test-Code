using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitVFX : MonoBehaviour
{
    public ParticleSystem defaultHitVFX;
    public ParticleSystem laserHitVFX;
    public int hitVFXEmission;
    public bool isEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default") && other.CompareTag("Untagged"))
        {
            PlayHitVFX();
        }

        if (!isEnemy)
        {
            if (other.CompareTag("Enemy"))
            {
                PlayHitVFX();
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                PlayHitVFX();
            }
        }
    }

    public void ManualHitVFXTtrigger()
    {
        PlayHitVFX();
    }

    public void PlayHitVFX()
    {
        if (defaultHitVFX.gameObject.activeInHierarchy)
        {
            defaultHitVFX.Emit(hitVFXEmission);
        }
        else if (laserHitVFX.gameObject.activeInHierarchy)
        {
            laserHitVFX.Emit(hitVFXEmission);
        }
    }
}
