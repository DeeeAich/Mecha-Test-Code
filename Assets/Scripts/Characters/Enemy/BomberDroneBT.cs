using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class BomberDroneBT : BehaviourTree
{
    public GameObject explosion;
    public float approachDistance, pauseTime, dashSpeed, dashAcceleration, explosionSize, explosionDamage;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player.transform.position);
        root = new RootNode(this,
            new Sequence(
                new Approach("player", approachDistance),
                new StoreValue("player", "destination"),
                new PauseFixed(pauseTime),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new MoveTo("destination"),
                new AITree.Detonate(explosionSize, explosionDamage, explosion),
                new PauseFixed(999f) //just to stop it from looping before death
                )
            );
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        AddOrOverwrite("player", player.transform.position);
    }
}

namespace AITree
{
    public class PackedBomber : Sequence
    {
        public PackedBomber(float approachDistance, float pauseTime, float dashSpeed, float dashAcceleration, float explosionSize, float explosionDamage, GameObject explosion) : base()
        {
            children = new List<Node>
                {
                new Approach("player", approachDistance),
                new StoreValue("player", "destination"),
                new PauseFixed(pauseTime),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new MoveTo("destination"),
                new AITree.Detonate(explosionSize, explosionDamage, explosion),
                new PauseFixed(999f) //just to stop it from looping before death
                    };
        }
    }
}