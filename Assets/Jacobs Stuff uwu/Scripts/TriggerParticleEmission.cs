using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticleEmission : MonoBehaviour
{
    public ParticleSystem ps;
    public int numberOfParticles = 1;

    public void TriggerParticle()
    {
        if (ps != null && ps.gameObject.activeInHierarchy) 
        { 
            ps.Emit(numberOfParticles); 
        }
    }
}
