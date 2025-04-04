using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DepreciaatedCode
{

    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBehaviour : MonoBehaviour
    {
        public List<MovementBehaviour> behaviours;
        internal NavMeshAgent agent;
        internal GameObject player;
        internal GameObject target;
        internal bool isShieldable = true;
        [SerializeField] internal int currentBehaviour;
        internal virtual void Start()
        {
            //Find the player
            player = GameObject.FindGameObjectWithTag("Player");
            target = player;
            agent = GetComponent<NavMeshAgent>();
            currentBehaviour = 0;
            behaviours[currentBehaviour].Enter();

            foreach (MovementBehaviour m in behaviours)
            {
                m.Setup();
                m.brain = this;
            }
        }

        // Update is called once per frame
        internal virtual void Update()
        {
            if (player == null)
            {
                return;
            }
            bool changed;
            int changeCount = 0;
            do
            {
                changed = false;
                int change = behaviours[currentBehaviour].CheckTransitionState();
                if (change != 0)
                {
                    changed = true;
                    behaviours[currentBehaviour].Exit();
                    currentBehaviour += change;
                    currentBehaviour += behaviours.Count;
                    currentBehaviour %= behaviours.Count;
                    behaviours[currentBehaviour].Enter();
                    changeCount++;
                }

                if (changeCount > behaviours.Count + 2)
                {
                    break;
                }
            }
            while (changed);
            behaviours[currentBehaviour].GetTargetLocation();
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
        internal EnemyBehaviour brain;

        public virtual void GetTargetLocation()
        {
            brain.agent.SetDestination(brain.gameObject.GetComponent<NavMeshAgent>().destination); //Hey, this is redundant, but the principle counts
        }

        public virtual int CheckTransitionState()
        {
            return 0;
        }
        public virtual void Enter()
        {
        }
        public virtual void Exit()
        {
        }
        public virtual void Setup()
        {
        }
    }

    public class GetEnemyShieldTarget : MovementBehaviour
    {
        readonly int failJump;
        readonly Shielder shielder;
        public GetEnemyShieldTarget(int failJump, Shielder shielder)
        {
            this.failJump = failJump;
            this.shielder = shielder;
        }

        public override int CheckTransitionState()
        {
            List<EnemyBehaviour> potentialTargets = new List<EnemyBehaviour>();
            potentialTargets.AddRange(GameObject.FindObjectsOfType<EnemyBehaviour>());
            if (potentialTargets.Count == 0)
                return failJump;
            for (int i = 0; i < potentialTargets.Count; i++)
            {
                if (!potentialTargets[i].isShieldable)
                {
                    potentialTargets.RemoveAt(i);
                    i--;
                }
            }
            if (potentialTargets.Count == 0)
                return failJump;
            int primeTarget = 0;
            float distToPrime = (brain.gameObject.transform.position - potentialTargets[primeTarget].gameObject.transform.position).sqrMagnitude;
            for (int i = 1; i < potentialTargets.Count; i++)
            {
                float potentialDist = (brain.gameObject.transform.position - potentialTargets[i].gameObject.transform.position).sqrMagnitude;
                if (potentialDist < distToPrime)
                {
                    primeTarget = i;
                    distToPrime = potentialDist;
                }
            }
            brain.target = potentialTargets[primeTarget].gameObject;
            shielder.SetTarget(brain.target);

            return 1;
        }
    }

    public class IndexJump : MovementBehaviour
    {
        readonly int jump;
        public IndexJump(int jump)
        {
            this.jump = jump;
        }

        public override int CheckTransitionState()
        {
            return jump;
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

        public override int CheckTransitionState()
        {
            if ((brain.gameObject.transform.position - brain.target.transform.position).magnitude > maxDist || (brain.gameObject.transform.position - activeTarget).magnitude < approachRange)
            {
                Exit();
                return 1;
            }
            return 0;
        }

        public override void GetTargetLocation()
        {
            if ((brain.gameObject.transform.position - activeTarget).magnitude > approachRange)
            {
                brain.agent.SetDestination(activeTarget);
                return;
            }
            Vector3 offset = brain.gameObject.transform.position - brain.target.transform.position;
            Vector3 randVect = UnityEngine.Random.insideUnitSphere;
            randVect.y = 0;
            randVect.Normalize();
            if (Vector3.Dot(randVect, offset) < 0)
            {
                randVect = -randVect;
            }
            randVect = (randVect.normalized * minDist + randVect * (maxDist - minDist));
            activeTarget = brain.target.transform.position + randVect;
            brain.agent.SetDestination(activeTarget);
        }

        public override void Enter()
        {
            base.Enter();
            activeTarget = brain.gameObject.transform.position;
            GetTargetLocation();
        }
    }

    public class ApproachUntilDistance : MovementBehaviour
    {
        public float distance = 0.01f;

        public ApproachUntilDistance(float distance)
        {
            this.distance = distance;
        }

        public override int CheckTransitionState()
        {
            if ((brain.gameObject.transform.position - brain.target.transform.position).magnitude <= distance)
            {
                return 1;
            }
            return 0;
        }

        public override void GetTargetLocation()
        {
            brain.agent.SetDestination(brain.target.transform.position);
        }
    }

    public class RemainStationaryInRange : MovementBehaviour
    {
        public float range = 10f;

        public RemainStationaryInRange(float range)
        {
            this.range = range;
        }

        public override void GetTargetLocation()
        {
            brain.agent.SetDestination(brain.gameObject.transform.position);
        }
        public override int CheckTransitionState()
        {
            if ((brain.gameObject.transform.position - brain.target.transform.position).magnitude >= range)
            {
                Exit();
                return 1;
            }
            return 0;
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
        public override void Enter()
        {
            timer = 0;
            brain.agent.isStopped = true;
        }

        public override void Exit()
        {
            brain.agent.isStopped = false;
            base.Exit();
        }

        public override int CheckTransitionState()
        {
            timer += Time.deltaTime;
            if (timer >= pauseLength)
            {
                Exit();
                return 1;
            }
            return 0;
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

        public override int CheckTransitionState()
        {
            timer += Time.deltaTime;
            if (timer >= pauseLength)
            {
                return 1;
            }
            return 0;
        }

        public override void Enter()
        {
            timer = 0;
            pauseLength = UnityEngine.Random.Range(minLength, maxLength);
            brain.agent.isStopped = true;
            Debug.Log(brain.gameObject.name + " entered Stun");
        }

        public override void Exit()
        {
            brain.agent.isStopped = false;
            Debug.Log("Escaped Stun " + timer + " over " + pauseLength);
        }

        public override void GetTargetLocation()
        {
            brain.agent.SetDestination(brain.agent.destination);
        }
    }

    public class ModifySpeed : MovementBehaviour
    {
        private readonly float newSpeed;

        public ModifySpeed(float newSpeed)
        {
            this.newSpeed = newSpeed;
        }

        public override void Enter()
        {
            brain.agent.speed = newSpeed;
        }

        public override int CheckTransitionState()
        {
            return 1;
        }
    }

    public class ModifyAcceleration : MovementBehaviour
    {
        private readonly float newAcc;

        public ModifyAcceleration(float newAcc)
        {
            this.newAcc = newAcc;
        }

        public override void Enter()
        {
            brain.agent.acceleration = newAcc;
        }

        public override int CheckTransitionState()
        {
            return 1;
        }
    }


    public class MoveToDestination : MovementBehaviour
    {
        public override int CheckTransitionState()
        {
            if (brain.agent.remainingDistance <= 0.01f)
            {
                Exit();
                return 1;
            }
            return 0;
        }
    }

    public class TakeCoverBehindTarget : MovementBehaviour
    {
        public float minDist, maxDist;
        public float approachRange = 0.001f;
        public Vector3 activeTarget;
        public float stepDist;
        public TakeCoverBehindTarget(float minDist, float maxDist, float stepDist)
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            this.stepDist = stepDist;
        }

        public TakeCoverBehindTarget(float minDist, float maxDist)
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            this.stepDist = 1f;
        }

        public override int CheckTransitionState()
        {
            if (brain.target == null)
            {
                return 1;
            }
            return 0;
        }

        public override void GetTargetLocation()
        {
            if ((brain.gameObject.transform.position - activeTarget).magnitude > approachRange)
            {
                brain.agent.SetDestination(activeTarget);
                return;
            }
            Vector3 offset = brain.gameObject.transform.position - brain.player.transform.position;
            Vector3 randVect = UnityEngine.Random.insideUnitSphere;
            randVect.y = 0;
            randVect.Normalize();
            if (Vector3.Dot(randVect, offset) < 0)
            {
                randVect = -randVect;
            }
            if (Vector3.Dot(randVect, offset) < Mathf.Cos(45f * Mathf.Deg2Rad))
            {
                Vector3 left = Vector3.Cross(randVect, Vector3.up);
                Vector3 right = Vector3.Cross(Vector3.up, randVect);
                float leftDot = Vector3.Dot(left, offset);
                float rightDot = Vector3.Dot(right, offset);
                if (leftDot > rightDot)
                    randVect = left;
                else
                    randVect = right;

            }

            randVect = (randVect.normalized * minDist + randVect * (maxDist - minDist));
            activeTarget = brain.target.transform.position + randVect;
            Debug.DrawLine(brain.gameObject.transform.position, activeTarget, Color.red, 5f);
            brain.agent.SetDestination(activeTarget);
            approachRange = Mathf.Max((brain.gameObject.transform.position - activeTarget).magnitude * (1 - stepDist), 0.01f);
        }

        public override void Enter()
        {
            base.Enter();
            activeTarget = brain.gameObject.transform.position;
            GetTargetLocation();
        }
    }
    public class DiscardTarget : MovementBehaviour
    {
        public override int CheckTransitionState()
        {
            return 1;
        }

        public override void Enter()
        {
            brain.target = brain.player;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void GetTargetLocation()
        {
            base.GetTargetLocation();
        }

        public override void Setup()
        {
            base.Setup();
        }
    }
}