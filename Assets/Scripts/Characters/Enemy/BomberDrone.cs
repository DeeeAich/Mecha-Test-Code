using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberDrone : EnemyBehaviour
{
    [SerializeField] float stopDistance = 2f;
    [SerializeField] float pauseTime = 1f;
    [SerializeField] float dashSpeed = 5f, dashAcceleration = 20f;

    // Start is called before the first frame update
    internal override void Start()
    {
        behaviours = new List<MovementBehaviour>
        {
            new ApproachUntilDistance(stopDistance),
            new PauseForFixedTime(pauseTime),
            new ModifySpeed(dashSpeed),
            new ModifyAcceleration(dashAcceleration),
            //move to target for time or distance
            new MoveToDestination(),
            new Detonate()
        };

        base.Start();
    }

    // Update is called once per frame
    internal override void Update()
    {
        base.Update();
    }
}
