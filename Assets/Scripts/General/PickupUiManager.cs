using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickupUiManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button[] uiButtons;

    private void Start()
    {
        animator.SetInteger("itemSelect", 0);
        
        
    }

    private void Update()
    {
        for (int i = 0; i < uiButtons.Length; i++)
        {
            if (EventSystem.current.currentSelectedGameObject == uiButtons[i])
            {
                animator.SetInteger("itemSelect", i);
            }
        }
    }
}
