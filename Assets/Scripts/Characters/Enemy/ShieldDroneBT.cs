using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;

public class ShieldDroneBT : BehaviourTree
{
    public Shielder shielder;
    public float coverRangeMin, coverRangeMax, coverStep;

    public GameObject explosion;
    public float approachDistance, pauseTime, dashSpeed, dashAcceleration, explosionSize, explosionDamage, dashAngularSpeed, maxHideAngle;

    public float unshieldStunTimeMin, unshieldStunTimeMax;

    internal override void Awake()
    {
        explosionDamage = GetComponent<EnemyStats>().damage;
        base.Awake();
        health.isShieldable = false;
        memory.Add("CoverTarget", transform.position);
        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new Sequence(
                new Fallback(
                    new HasVariable("CoverTarget", typeof(Vector3)),
                    new GetShieldableEnemy(shielder, "ShieldTarget"),
                    new ShieldBomber(approachDistance, pauseTime, dashSpeed, dashAcceleration, dashAngularSpeed, explosionSize, explosionDamage, explosion, ExplosionEffect.noEffect, shielder, "ShieldTarget")
                    ),
                new TakeCover("CoverTarget", coverRangeMin, coverRangeMax, coverStep)
                )) ;
    }

    internal override void FixedUpdate()
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
    }

    private void OnEnable()
    {
        shielder.breakEvent.AddListener(ShieldBreak);
    }


    private void OnDisable()
    {
        shielder.breakEvent.RemoveListener(ShieldBreak);
    }

    internal void Stun()
    {
        StopForTime(Random.Range(unshieldStunTimeMin, unshieldStunTimeMax));
        root.Restart();
        memory.Clear();
        AddOrOverwrite("player", player);
    }

    internal void ShieldBreak()
    {
        memory.Clear();
        AddOrOverwrite("player", player);
        root.Restart();
        
    }


    internal override void Die()
    {
        memory.Clear();
        shielder.Break();
        base.Die();
    }
}


