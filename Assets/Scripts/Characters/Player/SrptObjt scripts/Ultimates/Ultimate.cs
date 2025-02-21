using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : PlayerPickup
{

    public float rechargeTime;
    public float castTime;
    public int[] damages;
    [Tooltip("Object to add to player")]
    public GameObject ultCaster;
    [Tooltip("Object to spawn for ult")]
    public GameObject ultObject;


    public virtual void ActivateUltimate()
    {



    }

    public virtual void EndUltimate()
    {



    }

    public virtual void UltUpdate()
    {



    }

}
