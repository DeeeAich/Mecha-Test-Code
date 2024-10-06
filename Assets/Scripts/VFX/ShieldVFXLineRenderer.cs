using System.Collections;
using System.Collections.Generic;
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

    public void ManualUpdate()
    {
        SetLinePositionsBetweenStartEnd();

        lineRenderer.SetPosition(0, lineStart.position);
        lineRenderer.SetPosition(1, point01.position);
        lineRenderer.SetPosition(2, point02.position);
        lineRenderer.SetPosition(3, point03.position);
        lineRenderer.SetPosition(4, lineEnd.position);
    }
    private void OnEnable()
    {
         StartCoroutine(TimeBetweenSetPos());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

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
