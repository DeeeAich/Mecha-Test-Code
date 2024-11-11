using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace AITree
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        public bool debug = false;
        public string mostRecentTick;

        public Dictionary<string, object> memory;
        public NavMeshAgent agent;
        public GameObject player;
        public RootNode root;
        public bool paused = false;
        internal bool isShieldable = true;

        public RootNode replacement;

        public virtual void AddOrOverwrite(string key, object o)
        {
            if (!memory.ContainsKey(key))
            {
                memory.Add(key, o);
            }
            else
            {
                memory[key] = o;
            }
        }

        public virtual void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            memory = new Dictionary<string, object>();
        }
        public virtual void FixedUpdate()
        {
            if (!paused)
            {
                Node.BehaviourTreeState status = root.Tick();
                if(status!=Node.BehaviourTreeState.RUNNING)
                {
                    root.Restart();
                }
                if(debug)
                {

                    Debug.Log(string.Format("{0} ticked {1} nodes this FixedUpdate",name , root.tickCounter));
                }
            }
        }

        private void LateUpdate()
        {
            if (replacement != null)
            {
                root = replacement;
                replacement = null;
            }
        }

        public virtual void Stop()
        {
            if (agent != null)
                agent.isStopped = true;
            paused = true;
        }

        public virtual void Resume()
        {
            if (agent != null)
                agent.isStopped = false;
            paused = false;
        }

        public virtual void StopForTime(float time)
        {
            paused = true;
            Stop();
            StartCoroutine(UnpauseAfter(time));
        }

        IEnumerator UnpauseAfter(float time)
        {
            float timer = 0;
            do
            {
                yield return null;
                timer += Time.deltaTime;
            } while (timer < time);
            paused = false;
            Resume();
        }
    }

    public abstract class Node
    {
        public enum BehaviourTreeState
        {
            NULL,
            RUNNING,
            SUCCESS,
            FAILURE
        }

        public BehaviourTree brain;
        public RootNode root;
        public Node parent;
        public List<Node> children;
        public BehaviourTreeState state = BehaviourTreeState.NULL;

        private bool setupChildren = true;

        protected Node()
        {
            children = new List<Node>();
        }

        protected Node(params Node[] children) : this()
        {
            foreach (Node n in children)
            {
                this.children.Add(n);
            }
        }


        public virtual BehaviourTreeState Tick()
        {
            root.tickCounter++;
            brain.mostRecentTick = ToString();
            if (state == BehaviourTreeState.NULL)
            {
                state = BehaviourTreeState.RUNNING;
                Begin();
            }
            return state;
        }

        public virtual void Restart()
        {
            state = BehaviourTreeState.NULL;
            foreach (Node child in children)
            {
                child.Restart();
            }
            return;
        }

        public virtual void Begin()
        {
            if (setupChildren)
            {
                foreach (Node child in children)
                {
                    child.brain = brain;
                    child.parent = this;
                    child.root = root;
                }
                setupChildren = false;
            }
        }

        public virtual void End()
        {

        }
    }

    public class RootNode : Node
    {
        public NavMeshAgent agent;
        public int tickCounter;
        public override BehaviourTreeState Tick()
        {
            tickCounter = 0;
            base.Tick();
            state = children[0].Tick();
            return state;
        }
        public RootNode(BehaviourTree brain, params Node[] children) : base(children)
        {
            this.brain = brain;
            root = this;
        }
    }

    public abstract class Decorator : Node
    {
        protected Decorator(Node child) : base(child)
        {

        }
    }

    //Inverter

    //Repeat

    //RepeatUntilFail

    //RepeatUntilSuccess

    public abstract class Control : Node
    {
        internal int childIndex = 0;
        protected Control(params Node[] children) : base(children)
        {
        }


        public override void Restart()
        {
            childIndex = 0;
            base.Restart();
        }
    }

    public class Sequence : Control
    {

        public Sequence(params Node[] children) : base(children)
        {
        }


        public override void End()
        {
            base.End();
        }

        public override void Restart()
        {
            base.Restart();
            childIndex = 0;
        }

        public override void Begin()
        {
            base.Begin();
            childIndex = 0;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();

            BehaviourTreeState childState;
            do
            {
                if (childIndex >= children.Count)
                {
                    state = BehaviourTreeState.SUCCESS;
                    return state;
                }
                childState = children[childIndex].Tick();
                switch (childState)
                {
                    case BehaviourTreeState.NULL:
                        break;
                    case BehaviourTreeState.RUNNING:
                        state = BehaviourTreeState.RUNNING;
                        break;
                    case BehaviourTreeState.SUCCESS:
                        state = BehaviourTreeState.RUNNING;
                        childIndex++;
                        break;
                    case BehaviourTreeState.FAILURE:
                        state = BehaviourTreeState.FAILURE;
                        break;
                    default:
                        break;
                }
            } while (childState == BehaviourTreeState.SUCCESS);
            return state;
        }
    }

    public class Fallback : Control
    {
        public Fallback(params Node[] children) : base(children)
        {

        }

        public override void Begin()
        {
            base.Begin();
            childIndex = 0;
        }

        public override void Restart()
        {
            childIndex = 0;
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();

            BehaviourTreeState childState;
            do
            {
                if (childIndex >= children.Count)
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
                childState = children[childIndex].Tick();
                switch (childState)
                {
                    case BehaviourTreeState.NULL:
                        break;
                    case BehaviourTreeState.RUNNING:
                        state = BehaviourTreeState.RUNNING;
                        break;
                    case BehaviourTreeState.SUCCESS:
                        state = BehaviourTreeState.SUCCESS;
                        break;
                    case BehaviourTreeState.FAILURE:
                        state = BehaviourTreeState.RUNNING;
                        childIndex++;
                        break;
                    default:
                        break;
                }
            } while (childState == BehaviourTreeState.FAILURE);
            return state;
        }
    }

    //Parallel?

    public abstract class Condition : Node
    {

        protected Condition()
        {
            children = new List<Node>();
        }
    }

    public abstract class Action : Node
    {
        protected Action()
        {
            children = new List<Node>();
        }
    }

    public class GetShieldableEnemy : Condition
    {
        readonly Shielder shielder;
        readonly string targetStore;
        public GetShieldableEnemy(Shielder shielder, string targetStore)
        {
            this.shielder = shielder;
            this.targetStore = targetStore;
        }

        public override void End()
        {
            base.End();
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();

            List<BehaviourTree> potentialTargets = new List<BehaviourTree>();
            potentialTargets.AddRange(GameObject.FindObjectsOfType<BehaviourTree>());
            if (potentialTargets.Count == 0)
            {
                brain.AddOrOverwrite(targetStore, null);
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            for (int i = 0; i < potentialTargets.Count; i++)
            {
                if (!potentialTargets[i].isShieldable)
                {
                    potentialTargets.RemoveAt(i);
                    i--;
                }
            }
            if (potentialTargets.Count == 0)
            {
                brain.AddOrOverwrite(targetStore, null);
                state = BehaviourTreeState.FAILURE;
                return state;
            }
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
            brain.AddOrOverwrite(targetStore, potentialTargets[primeTarget].gameObject);
            shielder.SetTarget(potentialTargets[primeTarget].gameObject);
            state = BehaviourTreeState.SUCCESS;
            return state;
        }

        
    }

    public class StrafeInRange : Action
    {
        public float minDist, maxDist;
        public float approachRange = 0.1f;
        public Vector3 activeTarget;
        readonly string targLoc;
        Vector3 target;

        public StrafeInRange(string targetLocation, float minDist, float maxDist) : base()
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            targLoc = targetLocation;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            target = (Vector3)brain.memory[targLoc];
            Vector3 distanceToDest = brain.gameObject.transform.position - activeTarget;
            distanceToDest.y = 0;
            Vector3 distanceToPlayer = brain.gameObject.transform.position - target;
            distanceToPlayer.y = 0;
            if ((distanceToPlayer).magnitude > maxDist)
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            if ((distanceToDest).magnitude < approachRange)
            {
                state = BehaviourTreeState.SUCCESS;
                return state;
            }
            state = BehaviourTreeState.RUNNING;
            return state;
        }


        public void GetTargetLocation()
        {
            Vector3 offset = brain.gameObject.transform.position - activeTarget;
            Vector3 randVect = Random.insideUnitSphere;
            randVect.y = 0;
            offset.y = 0;
            if (Vector3.Dot(randVect.normalized, offset.normalized) < 0)
            {
                randVect = -randVect;
            }
            randVect = (randVect.normalized * minDist + randVect * (maxDist - minDist));
            activeTarget += randVect;
            brain.agent.SetDestination(activeTarget);
        }

        public override void Begin()
        {
            base.Begin();
            target = (Vector3)brain.memory[targLoc];
            activeTarget = target;
            GetTargetLocation();
        }

    }

    public class Approach : Action
    {
        readonly string targetLocation;
        readonly float approachDist;
        Vector3 activeTarget;
        public Approach(string targetLocation, float approachDist) : base()
        {
            this.targetLocation = targetLocation;
            this.approachDist = approachDist;
        }

        public override void Begin()
        {
            base.Begin();
            state = BehaviourTreeState.RUNNING;

            
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick(); 
           
            if (brain.memory.TryGetValue(targetLocation, out object recovered))
            {
                activeTarget = (Vector3)recovered;
                brain.agent.SetDestination(activeTarget);
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
            }

            Vector3 distanceFrom = brain.gameObject.transform.position - activeTarget;
            distanceFrom.y = 0;
            if (state == BehaviourTreeState.FAILURE)
            {
                return state;
            }
            if ((distanceFrom).magnitude <= approachDist)
            {
                state = BehaviourTreeState.SUCCESS;
            }
            return state;
        }
    }

    public class PauseFixed : Action
    {
        readonly float pauseTime;
        float timer;
        public PauseFixed(float pauseTime) : base()
        {
            this.pauseTime = pauseTime;
        }

        public override void Begin()
        {
            base.Begin();
            timer = 0;
            state = BehaviourTreeState.RUNNING;
            brain.agent.isStopped = true;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            timer += Time.deltaTime;
            if (timer >= pauseTime)
            {
                state = BehaviourTreeState.SUCCESS;
                brain.agent.isStopped = false;
            }
            return state;
        }
    }

    public class PauseRandom : Action
    {
        readonly float minTime, maxTime;
        float timer;
        float waitTime;

        public PauseRandom(float min, float max) : base()
        {
            minTime = min;
            maxTime = max;
        }

        public override void Begin()
        {
            base.Begin();
            waitTime = Random.Range(minTime, maxTime);
            timer = 0;
            brain.agent.isStopped = true;
            state = BehaviourTreeState.RUNNING;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                state = BehaviourTreeState.SUCCESS;
                brain.agent.isStopped = false;
            }
            return state;
        }
    }


    public class ModifyAgentStat : Condition
    {
        readonly string stat;
        readonly object newValue;
        [Tooltip("stat can be: 1. speed 2. acceleration")]
        public ModifyAgentStat(string stat, object newValue) : base()
        {
            this.stat = stat;
            this.newValue = newValue;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = BehaviourTreeState.FAILURE;
            switch (stat)
            {
                case "speed":
                    if (newValue is float extractedSpeed)
                    {
                        brain.agent.speed = extractedSpeed;
                        state = BehaviourTreeState.SUCCESS;
                    }
                    break;
                case "acceleration":
                    if (newValue is float extractedAcceleration)
                    {
                        brain.agent.acceleration = extractedAcceleration;
                        state = BehaviourTreeState.SUCCESS;
                    }
                    break;
                default:
                    Debug.Log(string.Format("Stat: {0} does not exist", stat));
                    break;
            }
            return state;
        }
    }


    public class Detonate : Action
    {
        readonly float size, damage;
        readonly GameObject explosion;

        public Detonate(float size, float damage, GameObject explosion) : base()
        {
            this.size = size;
            this.damage = damage;
            this.explosion = explosion;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            brain.GetComponent<Health>().TriggerDeath();
            GameObject boom = Object.Instantiate(explosion, brain.transform.position, Quaternion.identity, null);
            Explosion spawned = boom.GetComponent<Explosion>();
            spawned.linearScale = size;
            spawned.damage = damage;
            return BehaviourTreeState.SUCCESS;
        }
    }

    public class MoveTo : Approach
    {
        public MoveTo(string targetLocation) : base(targetLocation, 0.5f)
        {
        }

    }

    public class TakeCover : Action
    {
        readonly string targetLoc;
        Vector3 target;
        Vector3 activeTarget;
        float approachRange = 0.001f;
        readonly float minDist, maxDist, stepDist;

        public TakeCover() : base()
        {
        }

        public TakeCover(string targetLoc, float minDist, float maxDist) : base()
        {
            this.targetLoc = targetLoc;
            this.minDist = minDist;
            this.maxDist = maxDist;
            stepDist = 1f;
        }
        public TakeCover(string targetLoc, float minDist, float maxDist, float stepDist) : this(targetLoc, minDist, maxDist)
        {
            this.stepDist = Mathf.Min(stepDist, 0.97f);
            this.stepDist = Mathf.Max(this.stepDist, 0.1f);
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (state == BehaviourTreeState.FAILURE)
            {
                return state;
            }
            Vector3 distance = brain.gameObject.transform.position - activeTarget;
            distance.y = 0;
            if ((distance).magnitude > approachRange)
            {
                brain.agent.SetDestination(activeTarget);
                state = BehaviourTreeState.RUNNING;
                return state;
            }
            state = BehaviourTreeState.SUCCESS;
            return state;
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override void Begin()
        {
            base.Begin();
            if (brain.memory.TryGetValue(targetLoc, out object recoveredTarget) && recoveredTarget is Vector3 recovered)
            {
                target = recovered;
                target.y = brain.transform.position.y;
                activeTarget = target;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                //Debug.Log("attemptedfail");
                return;
            }

            Vector3 offset = target - brain.player.transform.position;
            Vector2 randVect2D = Random.insideUnitCircle;
            Vector3 randVect = new Vector3(randVect2D.x, 0, randVect2D.y);
            offset.y = 0;
            randVect.Normalize();

            //Debug.DrawLine(brain.transform.position, brain.transform.position + randVect, Color.blue, 5f);
            //Debug.DrawLine(brain.player.transform.position, brain.player.transform.position + offset, Color.green, 5f);
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
            activeTarget = target + randVect;
            activeTarget.y = brain.transform.position.y;
            //Debug.DrawLine(brain.gameObject.transform.position, activeTarget, Color.red, 5f);
            brain.agent.SetDestination(activeTarget);
            approachRange = Mathf.Max((brain.gameObject.transform.position - activeTarget).magnitude * (1 - stepDist), 0.01f);
        }

        public override void End()
        {
            base.End();
        }
    }

    public class DiscardTarget : Condition
    {
        public DiscardTarget()
        {
        }
    }

    public class HasVariable : Condition
    {
        readonly string varName;
        readonly System.Type varType;
        public HasVariable(string variableName, System.Type variableType) : base()
        {
            varName = variableName;
            varType = variableType;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (brain.memory.TryGetValue(varName, out object recoveredVal) && recoveredVal.GetType() == varType)
            {
                if (!varType.IsClass || recoveredVal != null)
                {
                    state = BehaviourTreeState.SUCCESS;
                    return state;
                }
            }
            state = BehaviourTreeState.FAILURE;
            return state;
        }
    }

    public class InRange : Condition
    {
        readonly string targetLocation;
        readonly float distance;
        GameObject target;

        public InRange(string targetLocation, float distance):base()
        {
            this.targetLocation = targetLocation;
            this.distance = distance;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            Vector3 dist = target.transform.position - brain.transform.position;
            dist.y = 0;
            if (dist.magnitude > distance)
                state = BehaviourTreeState.FAILURE;
            else
                state = BehaviourTreeState.SUCCESS;
            return state;
        }

        public override void Begin()
        {
            base.Begin();
            target = brain.memory[targetLocation] as GameObject;
        }
    }

    public class StoreValue : Action
    {
        readonly string current, future;

        public StoreValue(string current, string future) : base()
        {
            this.current = current;
            this.future = future;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            brain.AddOrOverwrite(future, brain.memory[current]);
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }
}