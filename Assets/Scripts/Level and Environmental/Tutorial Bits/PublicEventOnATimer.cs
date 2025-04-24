using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PublicEventOnATimer : MonoBehaviour
{
    public UnityEvent Event;
    public float timeToWait;
    [SerializeField] private float timer;
    
    
    public void StartTimer()
    {
        timer = timeToWait;
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                Event.Invoke();
            }
        }
    }
}
