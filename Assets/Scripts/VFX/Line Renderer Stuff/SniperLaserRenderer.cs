using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLaserRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask layerMask;


    void FixedUpdate()
    {

        lineRenderer.SetPosition(0, transform.position);
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
     
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * 100);
        }
    }
}
