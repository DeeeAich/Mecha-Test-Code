using AITree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerBT : BehaviourTree
{
    [Header("Gizmo Settings")]
    [SerializeField] [Range(1, 100)] internal int arcSteps = 8;
    [SerializeField] internal Color[] attackColours;
    [SerializeField] internal Vector3 gizmoYOffset = new Vector3(0f, 0.01f, 0f);

    [Header("Others")]

    //run 2 trees in parallel, phases are kept aside to be swapped out, or in their own control node selector based on hp
    Node weaponsBrain, motionBrain;
    Node secondPhaseBrain;
    Node transitionBrain;

    [SerializeField] float transitionThreshold = 300f;

    [SerializeField] OverseerNailGun[] guns;

    [SerializeField] GameObject weaponsPivot;

    [SerializeField] float approachDist = 10f;
    [SerializeField] float biggestRange = 15f;
    [SerializeField] float chargeSpeed = 10f;
    [SerializeField] GameObject chargeDamageZone;
    [SerializeField] float targetOffsetAngle = 30f;
    [SerializeField] float offsetDistance = 10f;

    [Header("Weight Calc Vars")]
    [SerializeField] float inwardMaxRange = 35f;
    [SerializeField] float inwardMinRange = 30f;
    [SerializeField] float wideMaxRange = 45f, wideMinRange = 15f;
    [SerializeField] float zigzagMaxRange = 20f, zigzagMinRange = 18f;
    [SerializeField] float circleMaxRange = 24f, circleMinRange = 22f;
    [SerializeField] float slamMaxRange = 5f, slamMinRange = 0f;
    [SerializeField] float chargeMaxRange = 5f, chargeMinRange = 0f;
    [SerializeField] [Range(0f, 90f)] float frontRadius = 90f;
    float frontRadiusRadians;
    [SerializeField] float chargePatienceWeightBuildup = 0.2f;
    [SerializeField] float chargePatienceWeightDropoff = 0.2f;
    float chargePatienceWeight = 0f;
    OverseerAnimationManager animManage;

    [SerializeField] OverseerWeaponsLook look;

    private void OnDrawGizmosSelected()
    {
        frontRadiusRadians = frontRadius * Mathf.Deg2Rad;
        if (player != null)
            Gizmos.DrawLine(player.transform.position, (memory.TryGetValue("targetOffset", out object val) ? player.transform.position + (Vector3)val : player.transform.position));
        for (int i = 0; i < attackColours.Length; i++)
        {
            Gizmos.color = attackColours[i];
            float radStep = Mathf.PI / (2f * (float)arcSteps);
            switch (i)
            {
                case 0:
                    DrawCurvedRegions(radStep, wideMinRange, wideMaxRange);
                    break;
                case 1:
                    DrawCurvedRegions(radStep, inwardMinRange, inwardMaxRange);
                    break;
                case 2:
                    DrawCurvedRegions(radStep, zigzagMinRange, zigzagMaxRange);
                    break;
                case 3:
                    DrawCurvedRegions(radStep, circleMinRange, circleMaxRange);
                    break;
                case 4:

                    //Gizmos.DrawLine(gizmoYOffset + transform.position + transform.right * slamMinRange, gizmoYOffset + transform.position + transform.right * slamMaxRange);
                    //Gizmos.DrawLine(gizmoYOffset + transform.position - transform.right * slamMinRange, gizmoYOffset + transform.position - transform.right * slamMaxRange);
                    for (int a = 0; a < 4 * arcSteps; a++)
                    {
                        Vector3 dir0 = Vector3.zero, dir1 = Vector3.zero;
                        float rad0 = (float)a * radStep;
                        float rad1 = (float)(a + 1) * radStep;
                        dir0.x = Mathf.Cos(rad0);
                        dir0.z = Mathf.Sin(rad0);
                        dir1.x = Mathf.Cos(rad1);
                        dir1.z = Mathf.Sin(rad1);
                        dir0 = transform.rotation * dir0;
                        dir1 = transform.rotation * dir1;
                        Gizmos.DrawLine(gizmoYOffset + transform.position + dir0 * slamMinRange, gizmoYOffset + transform.position + dir1 * slamMinRange);
                        Gizmos.DrawLine(gizmoYOffset + transform.position + dir0 * slamMaxRange, gizmoYOffset + transform.position + dir1 * slamMaxRange);
                    }
                    break;
                case 5:
                    DrawCurvedRegions(radStep, chargeMinRange, chargeMaxRange, true);
                    break;
                case 6:
                    //Draw the Follow Laser Attack Pattern
                    break;
                default:
                    break;
            }
        }


    }


    private void DrawCurvedRegions(float radStep, float minRange, float maxRange, bool halfAngle = false)
    {
        float useAngle = halfAngle ? frontRadiusRadians / 2 : frontRadiusRadians;
        Vector3 rightLineBoundary = Vector3.zero;
        rightLineBoundary.z = Mathf.Cos(useAngle);
        rightLineBoundary.x = Mathf.Sin(useAngle);
        rightLineBoundary = transform.rotation * rightLineBoundary;
        Vector3 leftLineBoundary = Vector3.zero;
        leftLineBoundary.z = -Mathf.Cos(useAngle);
        leftLineBoundary.x = Mathf.Sin(useAngle);
        leftLineBoundary = transform.rotation * leftLineBoundary;
        //Draw default attack (2)
        Gizmos.DrawLine(gizmoYOffset + transform.position + rightLineBoundary * minRange, gizmoYOffset + transform.position + rightLineBoundary * maxRange);
        Gizmos.DrawLine(gizmoYOffset + transform.position - leftLineBoundary * minRange, gizmoYOffset + transform.position - leftLineBoundary * maxRange);
        leftLineBoundary = -leftLineBoundary;
        for (int a = 0; a < 2 * arcSteps; a++)
        {
            if ((a + 1) * radStep < Mathf.PI / 2 - useAngle || (a) * radStep > Mathf.PI / 2 + useAngle)
                continue;

            Vector3 dir0 = Vector3.zero, dir1 = Vector3.zero;
            float rad0 = (float)a * radStep;
            float rad1 = (float)(a + 1) * radStep;
            dir0.x = Mathf.Cos(rad0);
            dir0.z = Mathf.Sin(rad0);
            dir1.x = Mathf.Cos(rad1);
            dir1.z = Mathf.Sin(rad1);
            dir0 = transform.rotation * dir0;
            dir1 = transform.rotation * dir1;

            if (a * radStep < Mathf.PI / 2 - useAngle)
                dir0 = rightLineBoundary;
            if ((a + 1) * radStep > Mathf.PI / 2 + useAngle)
                dir1 = leftLineBoundary;

            Gizmos.DrawLine(gizmoYOffset + transform.position + dir0 * minRange, gizmoYOffset + transform.position + dir1 * minRange);
            Gizmos.DrawLine(gizmoYOffset + transform.position + dir0 * maxRange, gizmoYOffset + transform.position + dir1 * maxRange);
        }
    }

    private void Start()
    {
        frontRadiusRadians = Mathf.Deg2Rad * frontRadius;
        StartCoroutine(ChargeWeightBuilding());
    }

    internal override void Awake()
    {
        base.Awake();

        look = GetComponentInChildren<OverseerWeaponsLook>();
        animManage = GetComponentInChildren<OverseerAnimationManager>();

        InitialiseBrains();

        AddOrOverwrite("player", player);
        root = new RootNode(this,
            new MultiRoot(motionBrain, weaponsBrain)
            );
    }

    void InitialiseBrains()
    {
        weaponsBrain =
            new Sequence(
                new RandomBranching(false,
                    new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithBool(look.Pause, true), new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 2), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool)), new CallVoidFunctionWithBool(look.Pause, false)), LaserWeightTwo),
                    new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithBool(look.Pause, true), new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 1), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool)), new CallVoidFunctionWithBool(look.Pause, false)), LaserWeightOne),
                    new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithBool(look.Pause, true), new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 3), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool)), new CallVoidFunctionWithBool(look.Pause, false)), LaserWeightThree),
                    new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithBool(look.Pause, true), new CallVoidFunctionWithInt(animManage.LaserPatternAttack, 4), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool)), new CallVoidFunctionWithBool(look.Pause, false)), LaserWeightFour),
                    new CalcingRandomChoice(new Sequence(new CallVoidFunctionWithInt(animManage.GroundSlamAttack, 1), new RepeatUntilSuccess(new BooleanFunction(CheckAnimBool))), SlamWeight),
                    new CalcingRandomChoice(new ChargeAttack(chargeSpeed, chargeDamageZone, "player", Facing), ChargeWeight)),
                new YieldTime(3f)

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

    float ChargeWeight()
    {
        float facingScore = Vector3.Dot(gameObject.transform.forward, (player.transform.position - transform.position).normalized);
        if (Mathf.Acos(facingScore) < frontRadiusRadians / 2)
            return 0f;
        return 0f;
    }

    float LaserWeightOne()
    {
        float calc;
        float distanceScore = 0f;
        float distToTarget = (player.transform.position - transform.position).magnitude;
        if (distToTarget > inwardMinRange && distToTarget <= inwardMaxRange)
        {
            distToTarget -= inwardMinRange;
            float rangeDif = inwardMaxRange - inwardMinRange;
            distanceScore = distToTarget / rangeDif;
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        if (Mathf.Acos(facingScore) < frontRadiusRadians)
            facingScore = 0;
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
        if (Mathf.Acos(facingScore) < frontRadiusRadians)
            facingScore = 0;
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
            distanceScore = 1 - (distToTarget / rangeDif);
        }
        float facingScore = Vector3.Dot(-weaponsPivot.transform.up, (player.transform.position - transform.position).normalized);
        if (Mathf.Acos(facingScore) < frontRadiusRadians)
            facingScore = 0;
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
        if (Mathf.Acos(facingScore) < frontRadiusRadians)
            facingScore = 0;
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

    bool Facing()
    {
        float facingScore = Vector3.Dot(gameObject.transform.forward, (player.transform.position - transform.position).normalized);
        return Mathf.Acos(facingScore) > frontRadiusRadians / 2;
    }


    IEnumerator ChargeWeightBuilding()
    {
        while (true)
        {
            if (Facing())
            {
                chargePatienceWeight = Mathf.Min(chargePatienceWeight + chargePatienceWeightBuildup * Time.deltaTime, 5f);
            }
            else
            {
                chargePatienceWeight = Mathf.Max(chargePatienceWeight - chargePatienceWeightDropoff * Time.deltaTime, 0f);
            }
            yield return null; 
        }
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
    public class CallVoidFunctionWithBool : Action
    {
        System.Action<bool> action;
        bool suppliedInt;

        public CallVoidFunctionWithBool(System.Action<bool> func, bool value)
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

    public class ToggleObject : Action
    {
        GameObject objToToggle;

        public ToggleObject(GameObject thing) : base()
        {
            objToToggle = thing;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = BehaviourTreeState.SUCCESS;
            objToToggle.SetActive(!objToToggle.activeSelf);
            return state;
        }
    }

    public class ChargeAttack : Sequence
    {
        public ChargeAttack(float chargeSpeed, GameObject damageZone, string target, System.Func<bool> facingFunc) : base()
        {
            children = new List<Node> {
            new ModifyAgentStat("speed", 0f), //no speed,
            new ModifyAgentStat("angularSpeed", 90f), //rotate base,
            new RepeatUntilSuccess(new BooleanFunction(facingFunc)),
            new ModifyAgentStat("angularSpeed", 0f), //no rotate,
            new ModifyAgentStat("speed", chargeSpeed), //big speed
            new ToggleObject(damageZone),
            new SavePositionOfObject(target, "chargeTarget"),
            new Approach("chargeTarget", 0f, StoreType.POSITION),
            new ToggleObject(damageZone),
            new ModifyAgentStat("angularSpeed", 30f), //reset vals
            new ModifyAgentStat("speed", 3.5f)
            };
        }
    }

    public class SavePositionOfObject : Action
    {
        string obj, sto;
        public SavePositionOfObject(string objectRef, string positionStorage)
        {
            obj = objectRef;
            sto = positionStorage;
        }

        public override BehaviourTreeState Tick()
        {
            if (brain.memory.TryGetValue(obj, out object o))
            {
                if (o is Transform)
                {
                    brain.AddOrOverwrite(sto, (o as Transform).position);
                }
                else if (o is GameObject)
                {
                    brain.AddOrOverwrite(sto, (o as GameObject).transform.position);
                }
                else
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
                state = BehaviourTreeState.SUCCESS;
                return state;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
        }
    }
}