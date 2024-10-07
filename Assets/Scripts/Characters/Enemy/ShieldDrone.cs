using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDrone : EnemyBehaviour
{
    [SerializeField] private Shielder shielder;

    [SerializeField] float stopDistance = 2f;
    [SerializeField] float coverRangeMin = 2f, coverRangeMax = 5f;
    [SerializeField] float stunTimeMin = 2f, stunTimeMax = 5f;
    [SerializeField] float pauseTime = 1f;
    [SerializeField] float dashSpeed = 5f, dashAcceleration = 20f;
    // Start is called before the first frame update
    internal override void Start()
    {
        isShieldable = false;
        behaviours = new List<MovementBehaviour>
        {
            new GetEnemyShieldTarget(3, shielder),
            //new TakeCoverBehindTarget(coverRangeMin, coverRangeMax),
            new IndexJump(7),

            //Get Shieldable Ally * how store it?
            //new GetShieldableTarget(this, failIndex)
            //if found
                //Go near enemy ////Take Cover Behind (make a line(ish) of player-shielded-me
                //Shield them
                //Jump to start
            //else
                //Enter Bomber Mode
                //Copy Bomber Behaviour

            new ApproachUntilDistance(stopDistance),
            new PauseForFixedTime(pauseTime),
            new ModifySpeed(dashSpeed),
            new ModifyAcceleration(dashAcceleration),
            //move to target for time or distance
            new MoveToDestination(),
            new Detonate(),
            //new Stunned(stunTimeMin, stunTimeMax)
        };
        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        
    }
}
