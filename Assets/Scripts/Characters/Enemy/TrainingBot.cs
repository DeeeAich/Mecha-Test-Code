using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingBot : EnemyBehaviour
{
    [SerializeField] private float ApproachDist, StrafeRangeMin, StrafeRangeMax, PauseMin, PauseMax;

    /*
    public TrainingBot()
    {   //Can't use variables in constructor (uses default values, not editor values)
        behaviours = new List<MovementBehaviour>();
        behaviours.Add(new ApproachUntilDistance(ApproachDist));
        behaviours.Add(new StrafeWithinRange(StrafeRangeMin,StrafeRangeMax));
        behaviours.Add(new PauseForRandTime(PauseMin, PauseMax));
    }
    */
    internal override void Start()
    {
        behaviours = new List<MovementBehaviour>
        {
            new ApproachUntilDistance(ApproachDist),
            new StrafeWithinRange(StrafeRangeMin, StrafeRangeMax),
            new PauseForRandTime(PauseMin, PauseMax)
        };
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
    }
}
