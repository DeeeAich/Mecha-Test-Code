using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVFXLineRendererManager : MonoBehaviour
{
    public List<ShieldVFXLineRenderer> shieldVFXLineRenderers;

    public GameObject shieldedTarget;



    void LateUpdate()
    {
        if (shieldedTarget != null || !shieldedTarget.activeInHierarchy)
        {
            for (int i = 0; i < shieldVFXLineRenderers.Count; i++)
            {
                if (!shieldVFXLineRenderers[i].isActiveAndEnabled) { shieldVFXLineRenderers[i].gameObject.SetActive(true); }
                shieldVFXLineRenderers[i].lineEnd.position = shieldedTarget.transform.position;
                shieldVFXLineRenderers[i].ManualUpdate();
            }

        }
        else
        {
            for (int i = 0; i < shieldVFXLineRenderers.Count; i++)
            {
                shieldVFXLineRenderers[i].gameObject.SetActive(false);              
            }
        }
    }
}
