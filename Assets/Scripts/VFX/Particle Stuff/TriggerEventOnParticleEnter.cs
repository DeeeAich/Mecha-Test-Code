using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventOnParticleEnter : MonoBehaviour
{
    ParticleSystem ps;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    public List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        for (int i = 0; i < numEnter; i++)
        {
            Debug.Log("holy moly");
        }

    }
}
