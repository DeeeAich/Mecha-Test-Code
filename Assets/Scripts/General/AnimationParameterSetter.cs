using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationParameterSetter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void SetBoolTrue(string name)
    {
        animator.SetBool(name,true);
    }

    public void SetBoolFalse(string name)
    {
        animator.SetBool(name, false);
    }
}
