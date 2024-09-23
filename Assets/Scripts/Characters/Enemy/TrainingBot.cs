using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingBot : EnemyBehaviour
{
    public TrainingBot()
    {
        behaviours = new List<MovementBehaviour>();
        behaviours.Add(new ApproachUntilDistance(9f));
        behaviours.Add(new StrafeWithinRange(5f,10f));
        behaviours.Add(new PauseForFixedTime(0.5f));
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
