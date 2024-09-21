using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntryTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private bool oneTimeTrigger = true;
    [SerializeField] private bool detecting = true;

    public UnityEvent trigger;
    public void OnTriggerEnter(Collider other)
    {
        if (detecting && other.tag == triggerTag)
        {
            trigger.Invoke();
            if (oneTimeTrigger) detecting = false;
        }
    }
}
