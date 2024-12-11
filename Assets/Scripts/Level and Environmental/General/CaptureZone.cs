using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CaptureZone : MonoBehaviour
{
    public UnityEvent onCapture;
    public bool isCaptured;
    public float captureProgress;
    public float captureProgressPerSecond = 0.01f;

    public bool playerPresent;
    public int enemiesPresent;

    [SerializeField] private LayerMask characterLayers;

    private void FixedUpdate()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.localScale/2, transform.forward, transform.rotation, 0, characterLayers, QueryTriggerInteraction.Collide);

        enemiesPresent = 0;
        playerPresent = false;
        
        for (int i = 0; i < hits.Length; i++)
        {
            Health health = hits[i].collider.gameObject.GetComponent<Health>();

            if (health.entityType == EntityType.PLAYER) playerPresent = true;

            if (health.entityType == EntityType.ENEMY)
            {
                enemiesPresent++;
            }
        }

        if (playerPresent && enemiesPresent == 0)
        {
            captureProgress += captureProgressPerSecond * Time.fixedDeltaTime;
        }

        if (captureProgress >= 1)
        {
            isCaptured = true;
            onCapture.Invoke();
        }
    }
}
