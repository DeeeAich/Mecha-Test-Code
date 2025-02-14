using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OverseerAnimationManager : MonoBehaviour
{
    public Animator groundSlamAnim;

    public Animator bodybackAnim;

    [Header("Phase transition")]
    public Animator basePhase2Anim;
    public Animator bodyPhase2Anim;


    [Header("Lasers")]
    public Animator laserTargetAnim;
    public GameObject laserTargetPos;
    public Animator laser1Anim;
    public Animator laser2Anim;

    public bool testfire;
    public int testnum;

    private void Update()
    {
        if (testfire)
        {
            PhaseTransition();
            testfire = false;
        }
    }

    /*
    1 = straight narrow and in            Distance from boss center = 40                Desired Player distance = 35
    2 = straight wide and out             Distance from boss center = 12                Desired Player distance = between 15-45
    3 = zigzag                            Distance from boss center = 12                Desired Player distance = 18
    4 = circle                            Distance from boss center = 22                Desired Player distance = 22
    */
    public void LaserPatternAttack(int attackType)
    {
        laser1Anim.SetTrigger("Fire");
        laser2Anim.SetTrigger("Fire");
        laserTargetAnim.enabled = true;
        laserTargetAnim.SetInteger("AttackID", attackType);
        laserTargetAnim.SetTrigger("Fire");
    }
    public void LaserTrackingAttack()
    {
        laser1Anim.SetTrigger("Fire");
        laser2Anim.SetTrigger("Fire");
        laserTargetAnim.enabled = false;
    }

    public void GroundSlamAttack(int phase)
    {
        if(phase == 1)
        {
            groundSlamAnim.SetTrigger("SlamOne");
        }
        else
        {
            groundSlamAnim.SetTrigger("SlamTwo");
        }
    }


    public void PhaseTransition()
    {
        basePhase2Anim.SetTrigger("PhaseTwo");
        bodyPhase2Anim.SetTrigger("PhaseTwo");
        bodybackAnim.SetTrigger("PhaseTwo");
        groundSlamAnim.SetTrigger("PhaseTwo");
    }

}
