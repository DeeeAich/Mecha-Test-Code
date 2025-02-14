using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerLaserAnimation : MonoBehaviour
{
    public Transform start;
    public Transform hitPoint;
    public Transform target;
    public LineRenderer aimLineRenderer;
    public LineRenderer laserLineRenderer;
    public GameObject LaserTop;
    public Collider hitPointCollider;

    public ParticleSystem targetParticle1;
    public ParticleSystem targetParticle2;

    public bool aimOn;
    public bool laserOn;

  

    private void Start()
    {
        var emmision1 = targetParticle1.emission;
        var emmision2 = targetParticle2.emission;
        emmision1.rateOverTime = 0;
        emmision2.rateOverTime = 0;
        aimOn = false;
        laserOn = false;
    }

    public void ToggleAimLine(int i)
    {
        if (i == 0)
        {
            aimOn = false;
        }
        else
        {
            aimOn = true;
        }
    }
    public void ToggleLaserLine(int i)
    {
        if (i == 0)
        {
            laserOn = false;
        }
        else
        {
            laserOn = true;
        }
    }

    public void ToggleCollider(int i)
    {
        if (i == 0)
        {
            hitPointCollider.enabled = false;
        }
        else
        {
            hitPointCollider.enabled = true;
        }
    }
    private void Update()
    {

        if (aimOn)
        {
            aimLineRenderer.enabled = true;
            LaserTop.transform.LookAt(target.position);

            RaycastHit hit;
            if (Physics.Raycast(start.position, start.TransformDirection(Vector3.down), out hit, Mathf.Infinity))

            {
                Debug.DrawRay(start.position, start.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                hitPoint.position = hit.point;
            }
            else
            {
                Debug.DrawRay(start.position, start.TransformDirection(Vector3.down) * 1000, Color.white);
                hitPoint.position = target.position;
            }

            aimLineRenderer.SetPosition(0, start.position);
            aimLineRenderer.SetPosition(1, hitPoint.position);
        }
        else
        {
            aimLineRenderer.enabled = false;
            aimLineRenderer.SetPosition(0, start.position);
            aimLineRenderer.SetPosition(1, hitPoint.position);
        }

        if (laserOn)
        {
            laserLineRenderer.enabled = true;
            LaserTop.transform.LookAt(target.position);

            RaycastHit hit;
            if (Physics.Raycast(start.position, start.TransformDirection(Vector3.down), out hit, Mathf.Infinity))

            {
                Debug.DrawRay(start.position, start.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                hitPoint.position = hit.point;
            }
            else
            {
                Debug.DrawRay(start.position, start.TransformDirection(Vector3.down) * 1000, Color.white);
                hitPoint.position = target.position;
            }

            laserLineRenderer.SetPosition(0, start.position);
            laserLineRenderer.SetPosition(1, hitPoint.position);
            var emmision1 = targetParticle1.emission;
            var emmision2 = targetParticle2.emission;
            emmision1.rateOverTime = 50;
            emmision2.rateOverTime = 50;

        }
        else
        {
            laserLineRenderer.enabled = false;


            laserLineRenderer.SetPosition(0, start.position);
            laserLineRenderer.SetPosition(1, hitPoint.position);
            var emmision1 = targetParticle1.emission;
            var emmision2 = targetParticle2.emission;
            emmision1.rateOverTime = 0;
            emmision2.rateOverTime = 0;
        }

        if (!laserOn && !aimOn)
        {
            LaserTop.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
