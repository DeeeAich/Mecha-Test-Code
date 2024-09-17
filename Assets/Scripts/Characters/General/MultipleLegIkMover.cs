using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class MultipleLegIkMover : MonoBehaviour
{
    [SerializeField] private GameObject[] legIks;

    [SerializeField] private int maxLegsMovingAtOnce = 1;
    [SerializeField] private float legMoveSpeed = 0.2f;
    [SerializeField] private float ikMoveStopDistance = 0.1f;
    
    [SerializeField] private float noMovementZoneRadius = 1;
    [SerializeField] private float guaranteedMovementZoneRadius = 2;
    [SerializeField] private float distanceAheadOfCenterScalar = 1;
    [SerializeField] private float distanceAheadOfCenterCap = 0.3f;

    [Header("Internal, dont touch")]
    [SerializeField] private Vector3[] legIkTargets;
    [SerializeField] private Vector3[] moveTargets;
    [SerializeField] private float[] moveSpeeds;
    [SerializeField] private GameObject[] zoneCenters;

    [SerializeField] private bool[] legMoving;
    private int legMovingCount;

    private Vector3 pastPosition;
    private float speed;

    private void Start()
    {
        CreatePositionsLists();

        pastPosition = transform.position;
    }
    
    private void CreatePositionsLists()
    {
        legMoving = new bool[legIks.Length];

        legIkTargets = new Vector3[legIks.Length];
        moveTargets = new Vector3[legIks.Length];
        moveSpeeds = new float[legIks.Length];
        zoneCenters = new GameObject[legIks.Length];

        for (int i = 0; i < legIks.Length; i++)
        {
            legMoving[i] = false;

            legIkTargets[i] = new Vector3();
            moveTargets[i] = new Vector3();
            zoneCenters[i] = new GameObject();

            zoneCenters[i].name = "leg zone " + i;
            zoneCenters[i].transform.parent = transform;

            legIkTargets[i] = legIks[i].transform.position;
            moveTargets[i] = legIks[i].transform.position;
            zoneCenters[i].transform.position = legIks[i].transform.position;
        }
    }

    private void FixedUpdate()
    {
        UpdateLegPositions();
    }

    private void UpdateLegPositions()
    {
        speed = Vector3.Distance(transform.position, pastPosition);
        pastPosition = transform.position;

        for (int i = 0; i < legIks.Length; i++)
        {
            legMovingCount = 0; // counts the moving legs
            for (int j = 0; j < legMoving.Length; j++)
            {
                if (legMoving[j]) legMovingCount++;
            }
            
            if (legMoving[i]) // moves the leg
            {
                legIkTargets[i] = Vector3.MoveTowards(legIkTargets[i], moveTargets[i], moveSpeeds[i]);

                if ((legIkTargets[i] - moveTargets[i]).magnitude < ikMoveStopDistance) // stops leg once finished moving
                {
                    legMoving[i] = false;
                }
            }
            else // checks if leg needs to move
            {
                float dist = (legIkTargets[i] - zoneCenters[i].transform.position).magnitude;
                
                if ((dist > noMovementZoneRadius && legMovingCount < maxLegsMovingAtOnce) || dist > guaranteedMovementZoneRadius)
                {
                    // starts moving leg
                    legMoving[i] = true;
                    moveSpeeds[i] = dist * legMoveSpeed;
                    moveTargets[i] = zoneCenters[i].transform.position + Vector3.ClampMagnitude(
                        (zoneCenters[i].transform.position - moveTargets[i]).normalized * moveSpeeds[i] * distanceAheadOfCenterScalar, noMovementZoneRadius * distanceAheadOfCenterCap);
                    
                }
            }
            
            legIks[i].transform.position = legIkTargets[i];
        }
    }



    private void OnDrawGizmosSelected()
    {
        if (zoneCenters != null && zoneCenters.Length > 0)
        {
            for (int i = 0; i < legIks.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(zoneCenters[i].transform.position, noMovementZoneRadius);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(zoneCenters[i].transform.position, guaranteedMovementZoneRadius);
                
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(moveTargets[i], legIkTargets[i] + Vector3.up * 0.2f);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(legIkTargets[i], legIkTargets[i] + Vector3.up * 0.2f);
            }
        }
    
    }
}
