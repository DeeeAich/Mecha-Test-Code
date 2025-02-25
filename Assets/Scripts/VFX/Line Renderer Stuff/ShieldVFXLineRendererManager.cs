using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVFXLineRendererManager : MonoBehaviour
{
    public List<ShieldVFXLineRenderer> shieldVFXLineRenderers;

    public GameObject shieldedTarget;
    public bool shieldToggle;
    private bool playSound;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// tell each line renderer script to turn on and track the target
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void LateUpdate()
    {
        if (shieldedTarget != null && shieldToggle)
        {
            for (int i = 0; i < shieldVFXLineRenderers.Count; i++)
            {
                if (!shieldVFXLineRenderers[i].isActiveAndEnabled) { shieldVFXLineRenderers[i].gameObject.SetActive(true); }
                shieldVFXLineRenderers[i].lineEnd.position = shieldedTarget.transform.position;
                shieldVFXLineRenderers[i].ManualUpdate();
            }

        }
        
        if (!shieldToggle)
        {
            for (int i = 0; i < shieldVFXLineRenderers.Count; i++)
            {
                shieldVFXLineRenderers[i].gameObject.SetActive(false);              
            }
        }
    }
}
