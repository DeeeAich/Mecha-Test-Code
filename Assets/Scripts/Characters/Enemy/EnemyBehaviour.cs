using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    public List<MovementBehaviour> behaviours;
    private NavMeshAgent agent;
    private GameObject player;
    // Start is called before the first frame update

    public int currentBehaviour;
    internal virtual void Start()
    {
        //Find the player
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        currentBehaviour = 0;
        behaviours[currentBehaviour].Enter(gameObject, player);

        foreach (MovementBehaviour m in behaviours)
        {
            m.Setup(gameObject, player);
        }
    }

    // Update is called once per frame
    internal virtual void Update()
    {
        if (player == null)
        {
            return;
        }
        int changeCount = 0;
        while (behaviours[currentBehaviour].CheckTransitionState(gameObject, player))
        {
            behaviours[currentBehaviour].Exit(gameObject, player);
            currentBehaviour += 1;
            currentBehaviour %= behaviours.Count;
            behaviours[currentBehaviour].Enter(gameObject, player);
            changeCount++;
            if (changeCount > behaviours.Count + 2)
            {
                break;
            }
        }
        agent.SetDestination(behaviours[currentBehaviour].GetTargetLocation(gameObject, player));
    }

    public virtual void Stop()
    {
        agent.isStopped = true;
    }

    public virtual void Resume()
    {
        agent.isStopped = false;
    }
}

public abstract class MovementBehaviour
{
    internal NavMeshAgent agent;


    public virtual Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        return self.GetComponent<NavMeshAgent>().destination;
    }

    public virtual bool CheckTransitionState(GameObject self, GameObject target)
    {
        return false;
    }
    public virtual void Enter(GameObject self, GameObject target)
    {
    }
    public virtual void Exit(GameObject self, GameObject target)
    {
    }
    public virtual void Setup(GameObject self, GameObject target)
    {
        agent = self.GetComponent<NavMeshAgent>();
    }
}

public class StrafeWithinRange : MovementBehaviour
{
    public float minDist, maxDist;
    public float approachRange = 0.1f;
    public Vector3 activeTarget;

    public StrafeWithinRange(float minDist, float maxDist)
    {
        this.minDist = minDist;
        this.maxDist = maxDist;
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return (self.transform.position - target.transform.position).magnitude > maxDist || (self.transform.position - activeTarget).magnitude < approachRange;
    }

    public override Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        if ((self.transform.position - activeTarget).magnitude > approachRange)
        {
            return activeTarget;
        }
        Vector3 offset = self.transform.position - target.transform.position;
        Vector3 randVect = UnityEngine.Random.insideUnitSphere;
        randVect.y = 0;
        if (Vector3.Dot(randVect, offset) < 0)
        {
            randVect = -randVect;
        }
        randVect = (randVect.normalized * minDist + randVect * (maxDist - minDist));
        activeTarget = target.transform.position + randVect;
        return activeTarget;
    }

    public override void Enter(GameObject self, GameObject target)
    {
        base.Enter(self, target);
        activeTarget = self.transform.position;
        GetTargetLocation(self, target);
    }
}

public class ApproachUntilDistance : MovementBehaviour
{
    public float distance = 0.01f;

    public ApproachUntilDistance(float distance)
    {
        this.distance = distance;
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return (self.transform.position - target.transform.position).magnitude <= distance;
    }

    public override Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        return target.transform.position;
    }
}

public class RemainStationaryInRange : MovementBehaviour
{
    public float range = 10f;

    public RemainStationaryInRange(float range)
    {
        this.range = range;
    }

    public override Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        return self.transform.position;
    }
    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return (self.transform.position - target.transform.position).magnitude >= range;
    }
}

public class PauseForFixedTime : MovementBehaviour
{
    public float pauseLength;

    public PauseForFixedTime(float pauseLength)
    {
        this.pauseLength = pauseLength;
    }

    float timer;
    public override void Enter(GameObject self, GameObject target)
    {
        timer = 0;
        agent.isStopped = true;
    }

    public override void Exit(GameObject self, GameObject target)
    {
        agent.isStopped = false;
        base.Exit(self, target);
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        timer += Time.deltaTime;
        return timer >= pauseLength;
    }
}

public class PauseForRandTime : MovementBehaviour
{
    private readonly float minLength;
    private readonly float maxLength;
    private float timer = 0f, pauseLength = 0;
    public PauseForRandTime(float minLength, float maxLength)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        timer += Time.deltaTime;
        return timer >= pauseLength;
    }

    public override void Enter(GameObject self, GameObject target)
    {
        timer = 0;
        pauseLength = UnityEngine.Random.Range(minLength, maxLength);
        agent.isStopped = true;
    }

    public override void Exit(GameObject self, GameObject target)
    {
        agent.isStopped = false;
    }

    public override Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        return agent.destination;
    }
}

public class ModifySpeed : MovementBehaviour
{
    private readonly float newSpeed;

    public ModifySpeed(float newSpeed)
    {
        this.newSpeed = newSpeed;
    }

    public override void Enter(GameObject self, GameObject target)
    {
        agent.speed = newSpeed;
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return true;
    }
}
public class ModifyAcceleration : MovementBehaviour
{
    private readonly float newAcc;

    public ModifyAcceleration(float newAcc)
    {
        this.newAcc = newAcc;
    }

    public override void Enter(GameObject self, GameObject target)
    {
        agent.acceleration = newAcc;
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return true;
    }
}

public class Detonate : MovementBehaviour
{

}

public class MoveToDestination : MovementBehaviour
{
    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        return agent.remainingDistance <= 0.01f;
    }
}