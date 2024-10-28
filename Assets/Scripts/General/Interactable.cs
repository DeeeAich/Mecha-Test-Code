using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool singleUse;
    public UnityEvent onInteract;
    
    public void TriggerInteraction()
    {
        onInteract.Invoke();
        if (singleUse) enabled = false;
    }
}
