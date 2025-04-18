using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using UnityEngine.UIElements;
using FMODUnity;

public class ScrapGrinder : MonoBehaviour
{
    public Animator sparks;
    public ParticleSystem scrap1;
    public ParticleSystem scrap2;
    public EventReference grindSFX;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayOneShotSFX(grindSFX,transform.position);
            sparks.SetTrigger("Active");
            scrap1.Emit(2);
            scrap2.Emit(2);
            Destroy(other.gameObject);

    }
}
