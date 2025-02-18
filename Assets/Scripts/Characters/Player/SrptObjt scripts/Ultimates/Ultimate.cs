using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : ScriptableObject
{

    public float rechargeTime;
    public bool recharging = false;
    public float castTime;
    public int[] damages;
    public GameObject ultCaster;
    public GameObject ultObject;
    public PlayerUltyControl myController;
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
