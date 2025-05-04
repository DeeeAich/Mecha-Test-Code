using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOffest : MonoBehaviour
{
    public Animator anim;

    private void OnEnable()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        if (anim != null)
        {
            anim.SetFloat("offset", Random.Range(0f, 1f));
        }

    }

    public void TriggerDestroyAnimation()
    {
        anim.SetTrigger("Destroy");
    }
    public void TriggerPauseAnimation()
    {
        anim.speed = 0f;
    }
}
