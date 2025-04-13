using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArenaAssetManager : MonoBehaviour
{
    public Animator phaseOneArena;
    public Animator fallingAnim;

    public bool test;
    void Update()
    {
        if (test)
        {
            StartTransition();
            test = false;
        }
    }

    public void StartTransition()
    {
        phaseOneArena.SetTrigger("Transition");
        fallingAnim.SetTrigger("Transition");
    }

}
