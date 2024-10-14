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
            new GetEnemyShieldTarget(3, shielder), // index 0
            new TakeCoverBehindTarget(coverRangeMin, coverRangeMax), // 1
            new IndexJump(8),// 2

            //Get Shieldable Ally * how store it?
            //new GetShieldableTarget(this, failIndex)
            //if found
                //Go near enemy ////Take Cover Behind (make a line(ish) of player-shielded-me
                //Shield them
                //Jump to start
            //else
                //Enter Bomber Mode
                //Copy Bomber Behaviour

            new DiscardTarget(),// 3
            new ApproachUntilDistance(stopDistance),// 4
            new PauseForFixedTime(pauseTime),// 5
            new ModifySpeed(dashSpeed),// 6
            new ModifyAcceleration(dashAcceleration),// 7
            //move to target for time or distance
            new MoveToDestination(),// 8
            new Detonate(),// 9
            new PauseForRandTime(stunTimeMin, stunTimeMax)// 10
        };
        base.Start();
    }

    private void OnEnable()
    {
        shielder.breakEvent.AddListener(Stun);
    }


    private void OnDisable()
    {
        shielder.breakEvent.RemoveListener(Stun);
    }

    internal void Stun()
    {
        currentBehaviour = behaviours.Count - 1;
    }
}
