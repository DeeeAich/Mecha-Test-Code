using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGridToggle : MonoBehaviour
{
    public bool exit;
    public Animator animator;


    private void OnEnable()
    {
            animator.SetBool("exit", exit);
    }
}
