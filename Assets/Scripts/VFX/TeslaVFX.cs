using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaVFX : MonoBehaviour
{
    [SerializeField] private GameObject zapFFXPrefab;

    public void ZapObject(GameObject target)
    {
        GameObject zap = Instantiate(zapFFXPrefab, transform);
        zap.transform.localPosition = Vector3.zero;
        zap.GetComponent<ShieldVFXLineRendererManager>().shieldedTarget = target;
    }
}
