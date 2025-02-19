using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Airstrike", menuName = "Player/Ultimates")]
public class Airstike : Ultimate
{

    private List<GameObject> missiles = new();
    public int enemiesToHit = 9;
    public float betweenShots = 0.2f;


    public override void ActivateUltimate()
    {
        if (recharging)
            return;

        if (myAnimator == null)
            myAnimator = ultCaster.GetComponent<Animator>();

        recharging = true;

        Firing();
        
    }

    private IEnumerator Firing()
    {

        myAnimator.SetTrigger("Fire");

        yield return new WaitForSeconds(castTime);

        foreach (GameObject missile in missiles)
        {

            missile.GetComponent<Animator>().SetTrigger("Fire");

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
