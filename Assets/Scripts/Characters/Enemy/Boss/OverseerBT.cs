using AITree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerBT : BehaviourTree
{
    //run 2 trees in parallel, phases are kept aside to be swapped out, or in their own control node selector based on hp
    Node weaponsBrain, motionBrain;

    internal override void Awake()
    {
        base.Awake();

        InitialiseBrains();

        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new MultiRoot(motionBrain, weaponsBrain)
            );
    }

    void InitialiseBrains()
    {
        weaponsBrain = new RandomBranching(false,
            new CalcingRandomChoice(new Sequence(), WeightOneCalc)
            
            //replace sequence with actual behaviour
            
            );

        motionBrain = new Sequence();
        //head empty open inside
    }

    float WeightOneCalc()
    {
        return 1f;
    }
}
