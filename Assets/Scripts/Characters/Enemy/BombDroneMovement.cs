using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDroneMovement : EnemyBehaviour
{
    public BombDroneMovement()
    {
        behaviours = new List<MovementBehaviour>();
        behaviours.Add(new ApproachUntilDistance(1f));
        behaviours.Add(new RemainStationaryInRange(10f));
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}
