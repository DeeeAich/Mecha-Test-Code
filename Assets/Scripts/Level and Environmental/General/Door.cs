using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public bool opened;
    public bool locked;
    
    [Header("Opening and closing")]
    public UnityEvent onOpen;
    public UnityEvent onClose;
    
    [Header("Lock states")]
    public UnityEvent onUnlock;
    public UnityEvent onLock;
    
    public UnityEvent onFailToOpen;


    public void OpenDoor()
    {
        if (locked)
        {
            onFailToOpen.Invoke();
        }
        else
        {
            opened = true;
            onOpen.Invoke();
        }
    }

    public void CloseDoor()
    {
        opened = false;
        onClose.Invoke();
    }

    public void UnlockDoor()
    {
        locked = false;
        onUnlock.Invoke();
    }

    public void LockDoor()
    {
        locked = true;
        onLock.Invoke();
    }
}
