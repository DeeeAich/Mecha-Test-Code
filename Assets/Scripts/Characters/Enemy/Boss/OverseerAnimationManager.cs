using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class OverseerAnimationManager : MonoBehaviour
{
    public bool animationIsPlaying;


    [FormerlySerializedAs("groundSlamAnim")] public Animator bodyAnim;

    public Animator bodybackAnim;

    [Header("Phase transition")]
    public Animator basePhase2Anim;
    public Animator bodyPhase2Anim;


    [Header("Lasers")]
    public Animator laserTargetAnim;
    public GameObject laserTargetPos;
    public Animator laser1Anim;
    public Animator laser2Anim;
    public FollowGameObjectDelayed laser1TargetTracker;
    public FollowGameObjectDelayed laser2TargetTracker;

    public LoopingAudioEventSender laserAudio1;
    public LoopingAudioEventSender laserAudio2;
    

    /*
    1 = straight narrow and in            Distance from boss center = 40                Desired Player distance = 35
    2 = straight wide and out             Distance from boss center = 12                Desired Player distance = between 15-45
    3 = zigzag                            Distance from boss center = 12                Desired Player distance = 18
    4 = circle                            Distance from boss center = 22                Desired Player distance = 22
    */
    public void LaserPatternAttack(int attackType)
    {
        laser1TargetTracker.pauseAnimation = true;
        laser2TargetTracker.pauseAnimation = true;

        StartCoroutine(Action(5.5f));
        laser1Anim.SetTrigger("Fire");
        laser2Anim.SetTrigger("Fire");
        laserTargetAnim.enabled = true;
        laserTargetAnim.SetInteger("AttackID", attackType);
        laserTargetAnim.SetTrigger("Fire");
    }
    public void LaserTrackingAttack()
    {
        StartCoroutine(Action(5.5f));
        laser1Anim.SetTrigger("Fire");
        laser2Anim.SetTrigger("Fire");
        laserTargetAnim.enabled = false;
    }

    public void GroundSlamAttack(int phase)
    {
        StartCoroutine(Action(3.5f));
        if (phase == 1)
        {
            bodyAnim.SetTrigger("SlamOne");
        }
        else
        {
            bodyAnim.SetTrigger("SlamTwo");
        }
    }

    public void PrepareChargeAttach()
    {
        bodyAnim.SetBool("Prepare Charge", true);
    }
    public void EnterChargeAttack()
    {
        bodyAnim.SetBool("Charge", true);
        bodyAnim.SetBool("Prepare Charge", false);
    }
    public void ExitChargeAttack()
    { 
        bodyAnim.SetBool("Charge", false);
    }

    public void PhaseTransition()
    {
        basePhase2Anim.SetTrigger("PhaseTwo");
        bodyPhase2Anim.SetTrigger("PhaseTwo");
        bodybackAnim.SetTrigger("PhaseTwo");
        bodyAnim.SetTrigger("PhaseTwo");
        StartCoroutine(Action(2.5f));
    }

    public void PlayDeathAnimation()
    {
        Debug.Log("BOSS SHOULD EXPLODE");
        animationIsPlaying = true;
        bodyAnim.SetTrigger("Death");
        bodybackAnim.SetTrigger("Death");
        laserAudio1.LoopingAudio(1);
        laserAudio2.LoopingAudio(1);
    }
    IEnumerator Action(float actionTime)
    {
        animationIsPlaying = true;
        yield return new WaitForSeconds(actionTime);
        animationIsPlaying = false;
        laser1TargetTracker.pauseAnimation = false;
        laser2TargetTracker.pauseAnimation = false;
    }

}
