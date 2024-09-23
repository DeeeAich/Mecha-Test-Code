using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] public List<MovementBehaviour> behaviours;
    public NavMeshAgent agent;
    public GameObject player;
    // Start is called before the first frame update

    public int currentBehaviour;
    public virtual void Start()
    {
        //Find the player
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        currentBehaviour = 0;
        behaviours[currentBehaviour].Enter(gameObject, player);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        int changeCount = 0;
        while (behaviours[currentBehaviour].CheckTransitionState(gameObject, player))
        {
            behaviours[currentBehaviour].Exit(gameObject, player);
            currentBehaviour += 1;
            currentBehaviour %= behaviours.Count;
            behaviours[currentBehaviour].Enter(gameObject, player);
            changeCount++;
            if(changeCount > behaviours.Count+2)
            {
                break;
            }
        }
        agent.SetDestination(behaviours[currentBehaviour].GetTargetLocation(gameObject, player));
    }
}

public abstract class MovementBehaviour
{
    public virtual Vector3 GetTargetLocation(GameObject self, GameObject target)
    {
        return self.transform.position;
    }

    public virtual bool CheckTransitionState(GameObject self, GameObject target)
    {
        return true;
    }
    public virtual void Enter(GameObject self, GameObject target)
    {
    }
    public virtual void Exit(GameObject self, GameObject target)
    {
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
    }

    public override bool CheckTransitionState(GameObject self, GameObject target)
    {
        timer += Time.deltaTime;
        return timer > pauseLength;
    }
}