using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticleEmission : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystem ps2nd;
    public int numberOfParticles = 1;
    public int numberOfParticles2nd = 1;

    public void TriggerParticle()
    {
        if (ps != null && ps.gameObject.activeInHierarchy) 
        { 
            ps.Emit(numberOfParticles); 
        }
    }
    public void Trigger2ndParticle()
    {
        if (ps2nd != null && ps2nd.gameObject.activeInHierarchy)
        {
            ps2nd.Emit(numberOfParticles2nd);
        }
    }
}
