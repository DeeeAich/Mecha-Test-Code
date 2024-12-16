using UnityEngine;
using System.Collections;

public class BeamParticles : MonoBehaviour
{

    private ParticleSystem[] particles;

    public float particlesPerUnit = 4;

    private void Start()
    {

        particles = GetComponentsInChildren<ParticleSystem>();

    }

    public void LaunchParticles(float distance)
    {

        ParticleSystem.ShapeModule[] newShapes = new ParticleSystem.ShapeModule[2];
        newShapes[0] = particles[0].shape;
        newShapes[1] = particles[1].shape;

        for (int i = 0; i < particles.Length - 1; i++)
        {
            newShapes[i].position = new Vector3(0, 0, distance / 2);
            newShapes[i].scale = new Vector3(newShapes[i].scale.x, newShapes[i].scale.y, distance);

            ParticleSystem.Burst burst = particles[i].emission.GetBurst(0);

            burst.count = distance * particlesPerUnit;

            particles[0].emission.SetBurst(0, burst);

        }
        
        

        foreach (ParticleSystem particle in particles)
            particle.Play();

    }

}

