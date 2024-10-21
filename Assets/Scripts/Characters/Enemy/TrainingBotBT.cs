using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class TrainingBotBT : BehaviourTree
{
    public float approachDist, strafeMin, strafeMax, pauseMin, pauseMax;

    public override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player.transform.position);
        root = new RootNode(this,
                   new Sequence(
                       new Approach("player", approachDist),
                       new StrafeInRange("player", strafeMin, strafeMax),
                       new PauseRandom(pauseMin, pauseMax)
                )
            );
    }

    override public void Update()
    {
        base.Update();
        AddOrOverwrite("player", player.transform.position);
    }
}
