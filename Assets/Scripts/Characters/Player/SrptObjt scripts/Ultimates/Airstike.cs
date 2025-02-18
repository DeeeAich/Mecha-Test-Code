using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Airstrike", menuName = "Player/Ultimates")]
public class Airstike : Ultimate
{

    private AITree.BehaviourTree[] enemies;
    public int enemiesToHit = 9;
    public float betweenShots = 0.2f;


    public override void ActivateUltimate()
    {
        if (recharging)
            return;

        if (myAnimator == null)
            myAnimator = ultCaster.GetComponent<Animator>();

        recharging = true;
        
    }

    private IEnumerator Firing()
    {

        myAnimator.SetTrigger("Fire");

        yield return new WaitForSeconds(castTime);

        foreach (AITree.BehaviourTree enemy in enemies)
        {

            GameObject launchedObject = GameObject.Instantiate(ultObject, enemy.transform);
            launchedObject.transform.SetParent(null);

            yield return new WaitForSeconds(betweenShots);

        }

        yield return null;
    }

    private IEnumerator ResetShot()
    {


        yield return new WaitForSeconds(rechargeTime);



        yield return null;

    }

    public override void EndUltimate()
    {
        
    }

    public override void UltUpdate()
    {
        
    }

}
