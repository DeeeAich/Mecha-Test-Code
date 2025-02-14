using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticleEmission : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystem ps2nd;
    public ParticleSystem ps3rd;
    public ParticleSystem ps4th;
    public int numberOfParticles = 1;
    public int numberOfParticles2nd = 1;
    public int numberOfParticles3rd = 1;
    public int numberOfParticles4th = 1;

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
    public void Trigger3rdParticle()
    {
        if (ps3rd != null && ps3rd.gameObject.activeInHierarchy)
        {
            ps3rd.Emit(numberOfParticles3rd);
        }
    }
    public void Trigger4thParticle()
    {
        if (ps4th != null && ps4th.gameObject.activeInHierarchy)
        {
            ps4th.Emit(numberOfParticles4th);
        }
    }
}
