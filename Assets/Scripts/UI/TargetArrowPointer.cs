using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrowPointer : MonoBehaviour
{
    public bool rotateTowards = true;
    public GameObject target;

    [SerializeField] private float screenEdgePadding = 20;

    private void OnEnable()
    {
        Update();
    }

    private void Update()
    {
        if (target == null) return;
        
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
        
        screenPoint = new Vector3(
            Mathf.Clamp(screenPoint.x, screenEdgePadding, Screen.width - screenEdgePadding),
            Mathf.Clamp(screenPoint.y, screenEdgePadding, Screen.height - screenEdgePadding),
            0);

        transform.position = screenPoint;

        if (rotateTowards)
        {
            float angle = Mathf.Atan2( ((Screen.height / 2) - screenPoint.y), ((Screen.width / 2) - screenPoint.x)) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0, angle + 90f);
        }
    }
}
