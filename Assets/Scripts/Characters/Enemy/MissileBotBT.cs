using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class MissileBotBT : BehaviourTree
{
    public float approachDist;
    public float peekStepSize = 2f;
    internal override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player);
        root = new RootNode(this,
                   new Fallback(
                       //new FindTarget("targetType", "target"),
                       new HasLineOfSight("player", PositionStoreType.GAMEOBJECT),
                       new Sequence(new FindLineOfSightPosition("LOSPOS", 5, peekStepSize, "player", PositionStoreType.GAMEOBJECT), new Approach("LOSPOS", 0.1f, PositionStoreType.VECTOR3)),
                       new ApproachUntilLineOfSight("player", approachDist)
                )
            );
    }

    override internal void FixedUpdate()
    {
        base.FixedUpdate();
    }
}