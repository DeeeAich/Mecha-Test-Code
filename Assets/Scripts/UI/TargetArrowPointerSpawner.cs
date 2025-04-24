using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrowPointerSpawner : MonoBehaviour
{
    public bool showArrow = true;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject targetArrowPrefab;
    
    [Header("Internal")]
    [SerializeField] private GameObject targetArrow;

    private void OnEnable()
    {
        if (PlayerUI.instance != null)
        {
            showArrow = true;
            targetArrow = Instantiate(targetArrowPrefab, PlayerUI.instance.transform);
            targetArrow.GetComponent<TargetArrowPointer>().target = target;
        }
    }

    private void FixedUpdate()
    {
        if(targetArrow != null && targetArrow.activeSelf != showArrow) targetArrow.SetActive(showArrow);
    }

    private void OnDisable()
    {
        if(targetArrow != null) Destroy(targetArrow);
    }

    private void OnDestroy()
    {
        if(targetArrow != null) Destroy(targetArrow);
    }

    public void ToggleArrow(bool active)
    {
        showArrow = active;
    }
}
