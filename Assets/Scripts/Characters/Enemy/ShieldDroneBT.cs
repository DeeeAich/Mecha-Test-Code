using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class ShieldDroneBT : BehaviourTree
{
    public Shielder shielder;
    public float coverRangeMin, coverRangeMax, coverStep;

    public GameObject explosion;
    public float approachDistance, pauseTime, dashSpeed, dashAcceleration, explosionSize, explosionDamage, dashAngularSpeed;

    public float unshieldStunTimeMin, unshieldStunTimeMax;

    public override void Awake()
    {
        isShieldable = false;
        base.Awake();
        memory.Add("CoverTarget", transform.position);
        root = new RootNode(this,
            new Sequence(
                new Fallback(
                    new HasVariable("CoverTarget", typeof(Vector3)),
                    new GetShieldableEnemy(shielder, "ShieldTarget"),
                    new PackedBomber(approachDistance, pauseTime, dashSpeed, dashAcceleration, dashAngularSpeed, explosionSize, explosionDamage, explosion, ExplosionEffect.noEffect)
                    ),
                new TakeCover("CoverTarget", coverRangeMin, coverRangeMax, coverStep)
                )) ;
    }

    override public void FixedUpdate()
    {
        if(memory.TryGetValue("ShieldTarget", out object o))
        {
            GameObject shieldTarget = o as GameObject;
            if(shieldTarget != null && shieldTarget.TryGetComponent<Health>(out Health hp) && hp.isAlive)
            {
                AddOrOverwrite("CoverTarget", shieldTarget.transform.position);
            }
            else
            {
                if (memory.ContainsKey("CoverTarget"))
                {
                    memory.Remove("CoverTarget");
                    //Debug.Log("removed covertarg");
                }
            }
        }
        else
        {
            if (memory.ContainsKey("CoverTarget"))
            {
                memory.Remove("CoverTarget");
            }
        }
        base.FixedUpdate();
        AddOrOverwrite("player", player.transform.position);
    }

    private void OnEnable()
    {
    }


    private void OnDisable()
    {
    }

    internal void Stun()
    {
        StopForTime(Random.Range(unshieldStunTimeMin, unshieldStunTimeMax));
        root.Restart();
        memory.Clear();
    }
}


