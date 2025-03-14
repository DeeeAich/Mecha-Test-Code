using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool singleUse;
    public bool canInteract = true;
    
    public UnityEvent onInteract;

    public void PlayerInRange(bool inRange)
    {
        if (inRange)
        {
            FindObjectOfType<PlayerBody>().SetInteract(this, true);
        }
        else
        {
            FindObjectOfType<PlayerBody>().SetInteract(this, false);
        }
    }
    public void TriggerInteraction()
    {
        if(!canInteract) return;
        
        onInteract.Invoke();
        
        if (singleUse)
        {
            FindObjectOfType<PlayerBody>().SetInteract(this, false);
            canInteract = false;
        }
    }
}
