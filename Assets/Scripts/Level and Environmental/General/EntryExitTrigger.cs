using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntryExitTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private bool disableAfterEntry = true;
    [SerializeField] private bool detecting = true;

    public UnityEvent entryTrigger;
    public UnityEvent exitTrigger;
    public void OnTriggerEnter(Collider other)
    {
        if (detecting && other.tag == triggerTag)
        {
            entryTrigger.Invoke();
            if (disableAfterEntry) detecting = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (detecting && other.tag == triggerTag)
        {
            exitTrigger.Invoke();
        }
    }
}
