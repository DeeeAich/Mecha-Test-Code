using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Airstrike", menuName = "Player/Ultimates")]
public class Airstike : Ultimate
{

    private AITree.BehaviourTree enemies;

    public override void ActivateUltimate()
    {
        if (recharging)
            return;

        AITree.BehaviourTree enemies = FindObjectOfType<AITree.BehaviourTree>();


    }

    public override void EndUltimate()
    {
        base.EndUltimate();
    }

    public override void UltUpdate()
    {
        base.UltUpdate();
    }

}
