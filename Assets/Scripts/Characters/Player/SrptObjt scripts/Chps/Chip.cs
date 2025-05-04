using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chip : PlayerPickup
{

    public enum ChipTypes
    {
        Body, Weapon,
        Movement
    }
    public enum ChipRarity
    {
        Common, Uncommon,
        Rare, UltraRare
    }
    [Header("Chip Type")]
    public ChipTypes chipType = ChipTypes.Body;

    public virtual bool HasMethod(UnityAction action, UnityAction checkAction)
    {

        if (action != null)
            foreach (Delegate del in action.GetInvocationList())
            {
                Debug.Log(del.Target == checkAction.Target);
                if(del.Target == checkAction.Target)
                    return true;
            }

        return false;
    }


}
