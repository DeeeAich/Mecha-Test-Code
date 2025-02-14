using AITree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerBT : BehaviourTree
{
    //run 2 trees in parallel, phases are kept aside to be swapped out, or in their own control node selector based on hp
    Node weaponsBrain, motionBrain;
    Node secondPhaseBrain;
    Node transitionBrain;

    [SerializeField] float approachDist = 10f;
    [SerializeField] float biggestRange = 15f;

    [SerializeField] float targetOffsetAngle = 30f;
    [SerializeField] float offsetDistance = 10f;


    private void OnDrawGizmosSelected()
    {
        if(player!=null)
        Gizmos.DrawLine(player.transform.position, (memory.TryGetValue("targetOffset", out object val) ? player.transform.position + (Vector3)val : player.transform.position));
    }

    internal override void Awake()
    {
        base.Awake();

        InitialiseBrains();

        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new MultiRoot(motionBrain, weaponsBrain)
            );
    }

    void InitialiseBrains()
    {
        weaponsBrain = new RandomBranching(false,
            new CalcingRandomChoice(new Sequence(), WeightOneCalc)
            
            //replace sequence with actual behaviour
            
            );

        motionBrain = new Sequence(
            new EnsureInRange("player", biggestRange, approachDist),
            new Selector(new BooleanFunction(ValidateOffset), new Sequence(new CalcOffsetTarget("targetOffset", "player", "moveLocation", offsetDistance), new MoveTo("moveLocation", StoreType.POSITION)), new GetOffset("player", "targetOffset", offsetDistance))

            );
        //head empty open inside
    }

    internal override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    float WeightOneCalc()
    {
        return 1f;
    }


    bool ValidateOffset()
    {
        Vector3 playerPos = Vector3.zero;
        Vector3 targetOffset = Vector3.zero;
        if (memory.TryGetValue("player", out object found))
        {
            if (found is GameObject)
            {
                playerPos = (found as GameObject).transform.position;
            }
            else if (found is Transform)
            {
                playerPos = (found as Transform).position;
            }
            else if (found is Vector3)
            {
                playerPos = (Vector3)found;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        if (memory.TryGetValue("targetOffset", out object foundOffset))
        {
            if (foundOffset is Vector3)
            {
                targetOffset = (Vector3)foundOffset;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        Vector3 currentOffset = transform.position - playerPos;
        float dot = Vector3.Dot(currentOffset.normalized, targetOffset.normalized);
        //cos(theta) = dot/(magnitudeA*magnitudeB) therefore when inputs are normalized: cos(theta) = dot/(1*1) = dot
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        return angle <= targetOffsetAngle;
    }
}


namespace AITree
{
    public class CalcOffsetTarget : Action
    {
        string offset, target, move;
        float distance;
        public CalcOffsetTarget(string offsetLocation, string targetLocation, string moveLocation, float distance)
        {
            offset = offsetLocation;
            target = targetLocation;
            move = moveLocation;
            this.distance = distance;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            GameObject playerPos = null;
            Vector3 targetOffset = Vector3.zero;
            if (brain.memory.TryGetValue(target, out object found))
            {
                if (found is GameObject)
                {
                    playerPos = (found as GameObject);
                }
                else if (found is Transform)
                {
                    playerPos = (found as Transform).gameObject;
                }
                else
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            if (brain.memory.TryGetValue(offset, out object foundOffset))
            {
                if (foundOffset is Vector3)
                {
                    targetOffset = (Vector3)foundOffset;
                }
                else
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }

            Vector3 targetPosition = playerPos.transform.position + (targetOffset.normalized * Mathf.Min(distance, (playerPos.transform.position - brain.transform.position).magnitude - 0.01f)) /*+ (playerPos.TryGetComponent<Rigidbody>(out Rigidbody rigid) ? rigid.velocity.normalized : Vector3.zero)*/;
            targetPosition.y = 0;
            brain.AddOrOverwrite(move, targetPosition);
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }
}