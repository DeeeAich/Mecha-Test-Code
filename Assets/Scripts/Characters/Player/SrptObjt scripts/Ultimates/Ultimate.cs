using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : ScriptableObject
{

    public float rechargeTime;
    public bool recharging = false;
    public float castTime;
    public int[] damages;
    [Tooltip("Object to add to player")]
    public GameObject ultCaster;
    [Tooltip("Object to spawn for ult")]
    public GameObject ultObject;
    public Animator myAnimator;


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
