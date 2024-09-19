using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationParameterSetter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator secondAnimator;
    [SerializeField] private bool isUsingSecondAnimator = false;


    public void SetBoolTrue(string name)
    {
        animator.SetBool(name,true);
        if (isUsingSecondAnimator)
        {
            secondAnimator.SetBool(name, true);
        }
    }

    public void SetBoolFalse(string name)
    {
        animator.SetBool(name, false);
        if (isUsingSecondAnimator)
        {
            secondAnimator.SetBool(name, false);
        }
    }
}
