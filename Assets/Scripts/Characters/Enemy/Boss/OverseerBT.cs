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

    [SerializeField] GameObject weaponsPivot;

    [SerializeField] float approachDist = 10f;
    [SerializeField] float biggestRange = 15f;

    [SerializeField] float targetOffsetAngle = 30f;
    [SerializeField] float offsetDistance = 10f;

    [Header("Weight Calc Vars")]
    [SerializeField] float inwardMaxRange = 35f; 
    [SerializeField] float inwardMinRange = 30f;
    [SerializeField] float wideMaxRange = 45f, wideMinRange = 15f;
    [SerializeField] float zigzagMaxRange = 20f, zigzagMinRange = 18f;
    [SerializeField] float circleMaxRange = 24f, circleMinRange = 22f;
    [SerializeField] float slamMaxRange = 5f, slamMinRange = 0f;

    OverseerAnimationManager animManage;

    private void OnDrawGizmosSelected()
    {
        if(player!=null)
        Gizmos.DrawLine(player.transform.position, (memory.TryGetValue("targetOffset", out object val) ? player.transform.position + (Vector3)val : player.transform.position));
    }

    internal override void Awake()
    {
        base.Awake();

        animManage = GetComponentInChildren<OverseerAnimationManager>();

        InitialiseBrains();

        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new MultiRoot(motionBrain, weaponsBrain)
            );
    }

    void InitialiseBrains()
    {
        weaponsBrain = new Sequence(new RandomBranching(false,
            new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 2), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), LaserWeightTwo),
            new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 1), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), LaserWeightOne),
            new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 3), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), LaserWeightThree),
            new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 4), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), LaserWeightFour),
            new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.GroundSlamAttack, 1), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), SlamWeight)
            ), new YieldTime(3f)

            //replace sequence with actual behaviour

            /*
            1 = straight narrow and in            Distance from boss center = 40                Desired Player distance = 35 //Maximum is 35, min is 30
            2 = straight wide and out             Distance from boss center = 12                Desired Player distance = between 15-45
            3 = zigzag                            Distance from boss center = 12                Desired Player distance = 18
            4 = circle                            Distance from boss center = 22                Desired Player distance = 22
            */
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

    float LaserWeightOne()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if(distToTarget > inwardMinRange && distToTarget <= inwardMaxRange)
        {
            distToTarget -= inwardMinRange;
            float rangeDif = inwardMaxRange - inwardMinRange;
            distanceScore = distToTarget / rangeDif;
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        calc = distanceScore * facingScore;
        return calc;
    }
    float LaserWeightTwo()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if (distToTarget > wideMinRange && distToTarget <= wideMaxRange)
        {
            float idealDist = (wideMaxRange + wideMinRange) / 2;
            distToTarget -= idealDist;
            distToTarget = Mathf.Abs(distToTarget);
            float offset = (wideMaxRange - idealDist) - distToTarget;
            distanceScore = offset / (wideMaxRange - idealDist);
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        calc = distanceScore * facingScore;
        return calc;
    }
    float LaserWeightThree()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if (distToTarget > inwardMinRange && distToTarget <= inwardMaxRange)
        {
            distToTarget -= inwardMinRange;
            float rangeDif = inwardMaxRange - inwardMinRange;
            distanceScore = 1- (distToTarget / rangeDif);
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        calc = distanceScore * facingScore;
        return calc;
    }
    float LaserWeightFour()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if (distToTarget > circleMinRange && distToTarget <= circleMaxRange)
        {
            float idealDist = (circleMaxRange + circleMinRange) / 2;
            distToTarget -= idealDist;
            distToTarget = Mathf.Abs(distToTarget);
            float offset = (circleMaxRange - idealDist) - distToTarget;
            distanceScore = offset / (circleMaxRange - idealDist);
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        calc = distanceScore * facingScore;
        return calc;
    }
    float SlamWeight()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if (distToTarget > slamMinRange && distToTarget <= slamMaxRange)
        {
            distToTarget -= slamMinRange;
            float rangeDif = slamMaxRange - slamMinRange;
            distanceScore = distToTarget / rangeDif;
        }
        //float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        calc = distanceScore;
        return calc;
    }

    bool CheckAnimBool()
    {
        return !animManage.animationIsPlaying;
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

    public class CallVoidFunctionWithInt : Action
    {
        System.Action<int> action;
        int suppliedInt;

        public CallVoidFunctionWithInt(System.Action<int> func, int value)
        {
            action = func;
            suppliedInt = value;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            action(suppliedInt);
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }
}