using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate : ScriptableObject
{

    public float rechargeTime;
    public bool recharging = false;
    public float castTime;
    public int[] damages;
    public GameObject ultimateCaster;
    public GameObject ultimateObject;
    public PlayerUltyControl myController;


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
