using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;
using Random = UnityEngine.Random;

public class ShieldVFXLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public Transform lineStart;
    public Transform point01;
    public Transform point02;
    public Transform point03;
    public Transform lineEnd;

    public Transform line01;
    public Transform line02;
    public Transform line03;

    public float timeBetweenSet;
    public float jitterDistance;

    public Material shieldedMaterial;
    public List<MeshRenderer> meshRenderers;
    private int[] meshRendererShieldIndexes;
    public bool materialSetter;     // only one of the line renderers needs this enabled 
    public EventReference attachSound;
    public EventReference detachSound;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// manual update called by parent script "ShieldVFXLineRendererManager"
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ManualUpdate()
    {
        SetLinePositionsBetweenStartEnd();

        lineRenderer.SetPosition(0, lineStart.position);
        lineRenderer.SetPosition(1, point01.position);
        lineRenderer.SetPosition(2, point02.position);
        lineRenderer.SetPosition(3, point03.position);
        lineRenderer.SetPosition(4, lineEnd.position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// on enable, set material and start jitter
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnEnable()
    {
        StartCoroutine(TimeBetweenSetPos());

        if (materialSetter)
        {
            AudioManager.instance.PlayOneShotSFX(attachSound, GetComponentInParent<ShieldVFXLineRendererManager>().shieldedTarget.transform.position);

            meshRenderers = new List<MeshRenderer>();


            // get mesh renderer on main object
            if (GetComponentInParent<ShieldVFXLineRendererManager>().shieldedTarget.GetComponent<MeshRenderer>() != null)
            {
                meshRenderers.Add(GetComponentInParent<ShieldVFXLineRendererManager>().shieldedTarget.GetComponent<MeshRenderer>());
            }

            // get mesh renderers childed to main object
            MeshRenderer[] childedMeshRenderers;
            childedMeshRenderers = GetComponentInParent<ShieldVFXLineRendererManager>().shieldedTarget.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < childedMeshRenderers.Length; i++)
            {
                meshRenderers.Add(childedMeshRenderers[i]);
            }

            meshRendererShieldIndexes = new int[meshRenderers.Count];

            SetAdditionalMaterial();
        }
    }

    public void SetAdditionalMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;

            bool needsToAddMaterial = true;

            for (int j = 0; j < meshRenderers[i].materials.Length; j++)
            {
                if (meshRenderers[i].materials[j].name == shieldedMaterial.name)
                {
                    needsToAddMaterial = false;
                }
            }

            if (needsToAddMaterial)
            {
                List<Material> materials = meshRenderers[i].materials.ToList();
                materials.Add(shieldedMaterial);
                meshRendererShieldIndexes[i] = materials.Count - 1;
                meshRenderers[i].materials = materials.ToArray();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// on disable, clear newly added material and stop jitter
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnDisable()
    {
        StopAllCoroutines();
        if (materialSetter)
        {
            ClearAdditionalMaterial();
            meshRenderers.Clear();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (materialSetter)
        {
            ClearAdditionalMaterial();
            meshRenderers.Clear();
        }
    }

    public void ClearAdditionalMaterial()
    {
        if (transform.parent.TryGetComponent<ShieldVFXLineRendererManager>(out ShieldVFXLineRendererManager manager) && manager.shieldedTarget != null)
        { 
            AudioManager.instance.PlayOneShotSFX(detachSound, manager.shieldedTarget.transform.position); 
            if(manager.shieldedTarget.TryGetComponent<Health>(out Health hp))
            {
                foreach(ShieldModifier sm in hp.damageMods)
                {
                    if (!sm.removeFlag)
                        return;
                }
            }
        }


        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;

            List<Material> materials = meshRenderers[i].materials.ToList();

            bool materialRemoved = false;
            for (int j = 0; j < materials.Count; j++)
            {
                if (materials[j].shader == shieldedMaterial.shader)
                {
                    materialRemoved = true;
                    materials.RemoveAt(j);
                    j--;
                }
            }

            if (materialRemoved) meshRenderers[i].materials = materials.ToArray();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// update the positions between us and the target
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void SetLinePositionsBetweenStartEnd()
    {
        line02.localPosition = GetCenterPoint(lineEnd.localPosition, lineStart.localPosition);
        line01.localPosition = GetCenterPoint(line02.localPosition, lineStart.localPosition);
        line03.localPosition = GetCenterPoint(lineEnd.localPosition, line02.localPosition);
    }
    public Vector3 GetCenterPoint(Vector3 A, Vector3 B)
    {
        Vector3 P = (B + A) / 2;
        return P;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// hawk tuah, jitter on that thang
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void SetLinePointRandomLocalPosition(Transform point)
    {
        point.localPosition = new Vector3(
            Random.Range(-jitterDistance, jitterDistance),
            Random.Range(-jitterDistance, jitterDistance),
            Random.Range(-jitterDistance, jitterDistance)
            );
    }

    IEnumerator TimeBetweenSetPos()
    {
        SetLinePointRandomLocalPosition(point01);
        SetLinePointRandomLocalPosition(point02);
        SetLinePointRandomLocalPosition(point03);
        yield return new WaitForSeconds(timeBetweenSet);

        StartCoroutine(TimeBetweenSetPos());
    }
}
