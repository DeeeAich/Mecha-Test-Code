using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public bool materialSetter;     // only one of the line renderers needs this enabled 

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

             SetAdditionalMaterial();
        }
    }

    public void SetAdditionalMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;
            
            List<Material> materials = meshRenderers[i].materials.ToList();
            materials.Add(shieldedMaterial);
            meshRenderers[i].materials = materials.ToArray();
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

    public void ClearAdditionalMaterial()
    {
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            if (meshRenderers[i] == null) continue;
            
            List<Material> materials = meshRenderers[i].materials.ToList();
            if (materials.Contains(shieldedMaterial)) materials.Remove(shieldedMaterial);
            meshRenderers[i].materials = materials.ToArray();
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// update the positions between us and the target
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void SetLinePositionsBetweenStartEnd()
    {
        line02.localPosition = GetCenterPoint(lineEnd.localPosition,lineStart.localPosition);
        line01.localPosition = GetCenterPoint(line02.localPosition,lineStart.localPosition);
        line03.localPosition = GetCenterPoint(lineEnd.localPosition,line02.localPosition);
    }
    public Vector3 GetCenterPoint(Vector3 A, Vector3 B)
    {
        Vector3 P = (B + A)/2;
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
