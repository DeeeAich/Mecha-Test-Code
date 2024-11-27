using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class TrainingBotBT : BehaviourTree
{
    public float approachDist, strafeMin, strafeMax, pauseMin, pauseMax;

    internal override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player);
        root = new RootNode(this,
                   new Sequence(
                       //new FindTarget("targetType", "target"),
                       new Approach("player", approachDist),
                       new StrafeInRange("player", strafeMin, strafeMax),
                       new PauseRandom(pauseMin, pauseMax)
                )
            );
    }

    override public void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
