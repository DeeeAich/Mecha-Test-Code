using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrapGrinder : MonoBehaviour
{
    public Animator sparks;
    public ParticleSystem scrap1;
    public ParticleSystem scrap2;

    private void OnTriggerEnter(Collider other)
    {

            sparks.SetTrigger("Active");
            scrap1.Emit(2);
            scrap2.Emit(2);
            Destroy(other.gameObject);

    }
}
