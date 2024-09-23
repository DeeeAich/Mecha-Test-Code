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
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (behaviours[currentBehaviour].CheckTransitionState(gameObject, player))
        {
            currentBehaviour += 1;
            currentBehaviour %= behaviours.Count;
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