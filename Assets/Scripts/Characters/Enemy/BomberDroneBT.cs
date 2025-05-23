using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITree;
using UnityEngine.Events;

public class BomberDroneBT : BehaviourTree
{
    public GameObject explosion;
    public float approachDistance, pauseTime, dashSpeed, dashAcceleration, explosionSize, explosionDamage, dashAngleSpeed;
    internal UnityEvent OnCharge;
    internal BomberLean lean;
    //bomber drone does short circuit dablage

    // Start is called before the first frame update
    internal override void Awake()
    {
        if (OnCharge == null)
            OnCharge = new UnityEvent();
        explosionDamage = GetComponent<EnemyStats>().damage;
        lean = GetComponentInChildren<BomberLean>();
        base.Awake();
        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new Sequence(
                new Approach("player", approachDistance),
                new InvokeEvent(OnCharge),
                new PauseFixed(pauseTime),
                new StoreValue("player", "destination", PositionStoreType.GAMEOBJECT, PositionStoreType.VECTOR3),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new ModifyAgentStat("angularSpeed", dashAngleSpeed),
                new MoveTo("destination", PositionStoreType.VECTOR3),
                new Detonate(explosionSize, explosionDamage, explosion, ExplosionEffect.hack),
                new PauseFixed(999f) //just to stop it from looping before death
                )
            );
    }

    private void OnEnable()
    {
        if (OnCharge == null)
            OnCharge = new UnityEvent();    
        OnCharge.AddListener(SetAnimation);
    }

    private void OnDisable()
    {
        OnCharge.RemoveListener(SetAnimation);
    }

    public void SetAnimation()
    {
        Animator anim = GetComponentInChildren<Animator>();
        if(anim !=null)
        anim.SetTrigger("Activate");
    }

    internal override void Die()
    {
        Destroy(lean);
    }

}

namespace AITree
{
    public class ShieldBomber : Sequence
    {
        public ShieldBomber(float approachDistance, float pauseTime, float dashSpeed, float dashAcceleration, float dashAngleSpeed, float explosionSize, float explosionDamage, GameObject explosion, ExplosionEffect effect, Shielder shielder, string shieldTarget) : base()
        {
            children = new List<Node>
                {
                new Approach("player", approachDistance),
                new Invert(new GetShieldableEnemy(shielder, shieldTarget)),
                new PauseFixed(pauseTime),
                new StoreValue("player", "destination",PositionStoreType.GAMEOBJECT, PositionStoreType.VECTOR3),
                new Invert(new GetShieldableEnemy(shielder, shieldTarget)),
                new ModifyAgentStat("speed", dashSpeed),
                new ModifyAgentStat("acceleration", dashAcceleration),
                new ModifyAgentStat("angularSpeed", dashAngleSpeed),
                new MoveTo("destination", PositionStoreType.VECTOR3),
                new Detonate(explosionSize, explosionDamage, explosion, effect),
                new PauseFixed(999f) //just to stop it from looping before death
                    };
        }
    }
}