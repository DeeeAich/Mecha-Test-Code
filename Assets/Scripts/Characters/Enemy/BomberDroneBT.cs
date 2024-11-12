using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class BomberDroneBT : BehaviourTree
{
    public GameObject explosion;
    public float approachDistance, pauseTime, dashSpeed, dashAcceleration, explosionSize, explosionDamage, dashAngleSpeed;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        AddOrOverwrite("player", player.transform.position);
        root = new RootNode(this,
            new Sequence(
                new Approach("player", approachDistance),
                new PauseFixed(pauseTime),
                new StoreValue("player", "destination"),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new ModifyAgentStat("angularSpeed", dashAngleSpeed),
                new MoveTo("destination"),
                new AITree.Detonate(explosionSize, explosionDamage, explosion, ExplosionEffect.hack),
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
        public PackedBomber(float approachDistance, float pauseTime, float dashSpeed, float dashAcceleration, float dashAngleSpeed, float explosionSize, float explosionDamage, GameObject explosion, ExplosionEffect effect) : base()
        {
            children = new List<Node>
                {
                new Approach("player", approachDistance),
                new StoreValue("player", "destination"),
                new PauseFixed(pauseTime),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new ModifyAgentStat("angularSpeed", dashAngleSpeed),
                new MoveTo("destination"),
                new Detonate(explosionSize, explosionDamage, explosion, effect),
                new PauseFixed(999f) //just to stop it from looping before death
                    };
        }
    }
}