using AITree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBotBT : BehaviourTree
{
    public float calmVariance, calmStepDistance, approachDist;

    internal override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player);
        root = new RootNode(this,
                   new Sequence(
                       //new FindTarget("targetType", "target"),
                       new Wander("CalmWanderMemory", calmVariance, calmStepDistance, approachDist)
                )
            );
    }

    override internal void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
