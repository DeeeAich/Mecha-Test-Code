using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace AITree
{

    public enum PositionStoreType
    {
        NULL,
        VECTOR3,
        TRANSFORM,
        GAMEOBJECT
    }

    public abstract class BehaviourTree : MonoBehaviour
    {
        public bool debug = false;
        public bool verboseDebug = false;

        public string mostRecentTick;
        public string tickPath;

        public Dictionary<string, object> memory;
        public NavMeshAgent agent;
        public GameObject player;
        public RootNode root;
        public bool paused = false;

        public RootNode replacement;
        CharacterVFXManager VFXManager;

        internal EnemyHealth health;

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
        private void OnDisable()
        {
            if (player != null)
                player.GetComponent<Health>().onDeath.RemoveListener(Stop);
        }
        internal virtual void Awake()
        {
            VFXManager = GetComponentInChildren<CharacterVFXManager>();
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Health>().onDeath.AddListener(Stop);
            agent = GetComponent<NavMeshAgent>();
            memory = new Dictionary<string, object>();
            health = GetComponent<EnemyHealth>();
        }
        internal virtual void FixedUpdate()
        {
            if (!paused)
            {
                tickPath = "";
                Node.BehaviourTreeState status = root.Tick();
                if (status != Node.BehaviourTreeState.RUNNING)
                {
                    root.Restart();
                }
                if (debug || verboseDebug)
                {

                    Debug.Log(string.Format("{0} ticked {1} nodes this FixedUpdate", name, root.tickCounter));
                }
                if (verboseDebug)
                {
                    Debug.Log(string.Format("{0} had {1} tick path", name, tickPath));
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
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
            paused = true;
            if (verboseDebug || debug)
            {
                Debug.Log(string.Format("{0} should not move.", name));
            }
        }

        public virtual void Resume()
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = false;
            paused = false;
            if (verboseDebug)
            {
                Debug.Log(string.Format("{0} should be able to move.", name));
            }
        }

        Coroutine pauseCoroutine;
        float pauseTimer;
        public virtual void StopForTime(float time)
        {
            paused = true;
            Stop();
            if (pauseCoroutine == null || pauseTimer <= 0f)
            {
                pauseCoroutine = StartCoroutine(UnpauseAfter(time));
            }
            else if (time > pauseTimer)
            {
                pauseTimer = time;
            }
        }


        IEnumerator UnpauseAfter(float time)
        {
            pauseTimer = 0;
            do
            {
                yield return null;
                pauseTimer += Time.deltaTime;
            } while (pauseTimer < time);
            paused = false;
            if (VFXManager != null)
            {
                VFXManager.ToggleEffectVFX(effect.ShortCircuit, false);
            }
            Resume();
        }
        internal virtual void Die()
        {
            Stop();
        }
    }

    #region basic Nodes
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
            brain.tickPath += string.Format("->{0}", ToString().Remove(0, 7));
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

    #endregion

    #region Decorators
    public abstract class Decorator : Node
    {
        protected Decorator(Node child) : base(child)
        {

        }
    }

    public class Invert : Decorator
    {
        public Invert(Node child) : base(child)
        {
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (children.Count == 0)
                state = BehaviourTreeState.FAILURE;
            else
            {
                BehaviourTreeState childState = children[0].Tick();
                switch (childState)
                {
                    case BehaviourTreeState.NULL:
                        break;
                    case BehaviourTreeState.RUNNING:
                        return BehaviourTreeState.RUNNING;
                    case BehaviourTreeState.SUCCESS:
                        return BehaviourTreeState.FAILURE;
                    case BehaviourTreeState.FAILURE:
                        return BehaviourTreeState.SUCCESS;
                }
            }
            return BehaviourTreeState.FAILURE;
        }
    }

    public class WeightedRandomChoice : Decorator
    {
        internal float weight;
        public virtual float Weight
        {
            get
            {
                return weight;
            }
            private set
            {
                weight = value;
            }
        }
        public WeightedRandomChoice(Node child) : base(child)
        {
        }

        public WeightedRandomChoice(Node child, float weight) : this(child)
        {
            Weight = weight;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = children[0].Tick();
            return state;
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void End()
        {
            base.End();
        }
    }

    public class CalcingRandomChoice : WeightedRandomChoice
    {
        readonly Func<float> calculate;
        public CalcingRandomChoice(Node child) : base(child)
        {
        }

        public CalcingRandomChoice(Node child, float weight) : base(child, weight)
        {
        }

        public CalcingRandomChoice(Node child, Func<float> calculation) : this(child)
        {
            calculate = calculation;
        }

        public override float Weight
        {
            get
            {
                if (calculate != null)
                    weight = calculate();
                return weight;
            }
        }
    }

    public class RepeatUntilSuccess : Decorator
    {
        public RepeatUntilSuccess(Node child) : base(child)
        {
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = children[0].Tick();
            if (state != BehaviourTreeState.SUCCESS)
            {
                state = BehaviourTreeState.RUNNING;
                return state;
            }
            return state;
        }
    }

    public class AlwaysSucceed : Decorator
    {
        public AlwaysSucceed(Node child) : base(child)
        {
        }
        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = children[0].Tick();
            if (state == BehaviourTreeState.RUNNING)
            {
                state = BehaviourTreeState.RUNNING;
                return state;
            }
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }

    //Repeat

    //RepeatUntilFail

    #endregion

    #region Control
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

    public class Selector : Control
    {
        int selectedIndex = -1;

        public Selector(Node Test, Node Success, Node Failure)
        {
            children = new List<Node> { Test, Success, Failure };
        }

        public override void Restart()
        {
            base.Restart();
            selectedIndex = -1;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();

            if (children.Count != 3)
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            else
            {
                BehaviourTreeState DeciderState = children[0].Tick();
                switch (DeciderState)
                {
                    case BehaviourTreeState.NULL:
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    case BehaviourTreeState.RUNNING:
                        state = BehaviourTreeState.RUNNING;
                        return state;
                    case BehaviourTreeState.SUCCESS:
                        selectedIndex = 1;
                        state = children[selectedIndex].Tick();
                        return state;
                    case BehaviourTreeState.FAILURE:
                        selectedIndex = 2;
                        state = children[selectedIndex].Tick();
                        return state;
                    default:
                        state = BehaviourTreeState.FAILURE;
                        return state;
                }
            }
        }
    }

    public class RandomBranching : Control
    {
        internal int lastIndex = -1;
        internal bool allowRepeat = true;
        public RandomBranching(params Node[] children) : base(children)
        {
        }

        public RandomBranching(params WeightedRandomChoice[] children) : base(children)
        {

        }
        public RandomBranching(bool repeats, params WeightedRandomChoice[] children) : this(children)
        {
            allowRepeat = repeats;
        }

        struct ChoiceData
        {
            internal WeightedRandomChoice w;
            internal float weight;
        }

        public override void Begin()
        {
            base.Begin();
            //choose now
            List<ChoiceData> data = new List<ChoiceData>();
            float weightTotal = 0f;
            foreach (WeightedRandomChoice w in children)
            {
                float childWeight = w.Weight;
                if (w.weight > 0.01f && (allowRepeat || children.IndexOf(w) != lastIndex))
                {
                    ChoiceData freshData = new ChoiceData();
                    freshData.w = w;
                    freshData.weight = childWeight;
                    data.Add(freshData);
                    weightTotal += childWeight;
                }
            }
            if (data.Count == 0)
            {
                if (allowRepeat)
                {
                    //Debug.LogWarning("Boss Move Selector Failed");
                    childIndex = 0;
                    return;
                }
                else
                {
                    allowRepeat = true;
                    weightTotal = 0f;
                    foreach (WeightedRandomChoice w in children)
                    {
                        float childWeight = w.Weight;
                        if (w.weight > 0.01f)
                        {
                            ChoiceData freshData = new ChoiceData();
                            freshData.w = w;
                            freshData.weight = childWeight;
                            data.Add(freshData);
                            weightTotal += childWeight;
                        }
                    }
                    if (data.Count == 0)
                    {
                        //Debug.LogWarning("Boss Move Selector Failed (even tried to repeat)");
                        childIndex = 0;
                        return;
                    }
                }
            }

            float choice = Random.Range(0f, weightTotal);
            float runningDecision = 0f;
            for (int i = 0; i < data.Count; i++)
            {
                if (runningDecision < choice && runningDecision + data[i].weight >= choice)
                {
                    childIndex = children.IndexOf(data[i].w);
                    break;
                }
                runningDecision += data[i].weight;
            }
        }

        public override void End()
        {
            base.End();
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = children[childIndex].Tick();
            return state;
        }
    }

    public class Parallel : Control
    {
        internal int successThresh = 0;
        int successCount = 0;
        int completeCount = 0;

        public Parallel(params Node[] children) : base(children)
        {
        }

        public Parallel(int threshold, params Node[] children) : this(children)
        {
            successThresh = threshold;
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void End()
        {
            base.End();
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            foreach (Node n in children)
            {
                if (n.state == BehaviourTreeState.RUNNING || n.state == BehaviourTreeState.NULL)
                {
                    n.Tick();
                    if (n.state == BehaviourTreeState.SUCCESS)
                    {
                        successCount++;
                        completeCount++;
                    }
                    else if (n.state == BehaviourTreeState.FAILURE)
                    {
                        completeCount++;
                    }
                }
            }
            if (completeCount >= children.Count)
            {
                state = BehaviourTreeState.FAILURE;
            }
            if (successCount >= successThresh)
            {
                state = BehaviourTreeState.SUCCESS;
            }
            return state;
        }
    }

    public class MultiRoot : Control
    {
        //parallel but restart childed nodes on completion
        public MultiRoot(params Node[] children) : base(children)
        {
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void End()
        {
            base.End();
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            foreach (Node n in children)
            {
                if (n.Tick() != BehaviourTreeState.RUNNING)
                {
                    n.Restart();
                }
            }
            return BehaviourTreeState.RUNNING;
        }
    }

    #endregion

    #region Conditions and Actions
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

    public class BooleanFunction : Condition
    {
        readonly Func<bool> calculate;
        readonly Func<string, bool> calcString;
        readonly string text;
        public BooleanFunction()
        {
        }
        public BooleanFunction(Func<bool> calculation) : this()
        {
            calculate = calculation;
        }
        public BooleanFunction(Func<string, bool> calculation, string text) : this()
        {
            calcString = calculation;
            this.text = text;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            bool result = calculate != null && calculate() || calcString != null && calcString(text);
            if (result)
            {
                state = BehaviourTreeState.SUCCESS;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
            }
            return state;
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

            List<EnemyHealth> potentialTargets = new List<EnemyHealth>();
            potentialTargets.AddRange(GameObject.FindObjectsOfType<EnemyHealth>());
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

    public class FindTarget : Action
    {
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0044
        string targetType;
        string storage;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044

        public FindTarget(string targetType, string storage)
        {
            this.targetType = targetType;
            this.storage = storage;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            return state;
        }
    }

    public class StrafeInRange : Action
    {
        public float minDist, maxDist;
        public float approachRange = 0.1f;
        public Vector3 activeTarget;
        public GameObject gameObjectTarget;
        public Transform transformTarget;
        readonly string targLoc;
        Vector3 target;

        readonly PositionStoreType type;

        public StrafeInRange(string targetLocation, float minDist, float maxDist) : base()
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            targLoc = targetLocation;
            type = PositionStoreType.GAMEOBJECT;
        }
        public StrafeInRange(string targetLocation, float minDist, float maxDist, PositionStoreType type) : this(targetLocation, minDist, maxDist)
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            targLoc = targetLocation;
            this.type = type;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (!brain.memory.TryGetValue(targLoc, out object recovered))
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            switch (type)
            {
                case PositionStoreType.NULL:
                    if (brain.debug)
                    {
                        Debug.Log("You had to put in effirt to end up here");
                    }
                    state = BehaviourTreeState.FAILURE;
                    return state;
                case PositionStoreType.GAMEOBJECT:
                    if (gameObjectTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        //Hvae I succeeded, or failed?
                        /*
                         If I want to stay near something until they die I have succeded if they die
                         If my follow up to strafe in range is like, kiss the target, then failure might be more appropriate, because target is gone

                         If I "StrafeInRange until failure" then loss of target being a failure is a good idea
                         */

                        return state;
                    }
                    target = gameObjectTarget.transform.position;
                    break;
                case PositionStoreType.VECTOR3:
                    target = (Vector3)recovered;
                    break;
                case PositionStoreType.TRANSFORM:
                    if (transformTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    }
                    target = transformTarget.position;
                    break;
                default:
                    state = BehaviourTreeState.FAILURE;
                    return state;
            }
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

            if (!brain.memory.TryGetValue(targLoc, out object recovered))
            {
                state = BehaviourTreeState.FAILURE;

            }

            switch (type)
            {
                case PositionStoreType.NULL:
                    target = new Vector3(0, 0, 0);
                    break;
                case PositionStoreType.GAMEOBJECT:
                    gameObjectTarget = (GameObject)recovered;
                    target = gameObjectTarget.transform.position;
                    break;
                case PositionStoreType.VECTOR3:
                    target = (Vector3)recovered;
                    break;
                case PositionStoreType.TRANSFORM:
                    transformTarget = (Transform)recovered;
                    target = transformTarget.position;
                    break;
                default:
                    target = new Vector3(0, 0, 0);
                    break;
            }
            activeTarget = target;
            GetTargetLocation();
        }

    }

    public class Approach : Action
    {
        readonly string targetLocation;
        readonly float approachDist;
        Vector3 activeTarget;
        GameObject objectTarget;
        Transform transformTarget;
        readonly PositionStoreType store;
        public Approach(string targetLocation, float approachDist) : base()
        {
            this.targetLocation = targetLocation;
            this.approachDist = approachDist;
            store = PositionStoreType.GAMEOBJECT;
        }

        public Approach(string targetLocation, float approachDist, PositionStoreType type) : this(targetLocation, approachDist)
        {
            store = type;
        }

        public override void Begin()
        {
            base.Begin();
            state = BehaviourTreeState.RUNNING;
            if (!brain.memory.TryGetValue(targetLocation, out object recovered))
            {
                state = BehaviourTreeState.FAILURE;
            }

            switch (store)
            {
                case PositionStoreType.NULL:
                    activeTarget = new Vector3(0, 0, 0);
                    state = BehaviourTreeState.FAILURE;
                    break;
                case PositionStoreType.GAMEOBJECT:
                    objectTarget = (GameObject)recovered;
                    if (objectTarget != null)
                        activeTarget = objectTarget.transform.position;
                    else
                    {
                        activeTarget = new Vector3(0, 0, 0);
                        state = BehaviourTreeState.FAILURE;
                    }
                    break;
                case PositionStoreType.VECTOR3:
                    activeTarget = (Vector3)recovered;
                    break;
                case PositionStoreType.TRANSFORM:
                    transformTarget = (Transform)recovered;
                    activeTarget = transformTarget.position;
                    break;
                default:
                    activeTarget = brain.transform.position;
                    break;
            }
            brain.agent.SetDestination(activeTarget);
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            switch (store)
            {
                case PositionStoreType.NULL:
                    break;
                case PositionStoreType.GAMEOBJECT:
                    if (objectTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        //Hvae I succeeded, or failed?
                        /*
                         If I want to stay near something until they die I have succeded if they die
                         If my follow up to strafe in range is like, kiss the target, then failure might be more appropriate, because target is gone

                         If I "StrafeInRange until failure" then loss of target being a failure is a good idea
                         */

                        return state;
                    }
                    activeTarget = objectTarget.transform.position;
                    brain.agent.SetDestination(activeTarget);
                    break;
                case PositionStoreType.VECTOR3:
                    if (brain.memory.TryGetValue(targetLocation, out object recovered))
                    {
                        activeTarget = (Vector3)recovered;
                    }
                    else
                    {
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    }
                    break;
                case PositionStoreType.TRANSFORM:
                    if (transformTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    }
                    activeTarget = transformTarget.position;
                    break;
                default:
                    state = BehaviourTreeState.FAILURE;
                    return state;
            }

            Vector3 distanceFrom = brain.gameObject.transform.position - brain.agent.destination;
            distanceFrom.y = 0;
            if (state == BehaviourTreeState.FAILURE)
            {
                return state;
            }
            if ((distanceFrom).magnitude <= approachDist)
            {
                state = BehaviourTreeState.SUCCESS;
                brain.agent.SetDestination(brain.transform.position);
            }


            if (brain.agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                if (brain.agent.velocity.magnitude < 0.01f)
                {
                    state = BehaviourTreeState.FAILURE;
                }
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
            if (brain.agent != null && brain.agent.isOnNavMesh)
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

    public class YieldTime : Action
    {
        float time;
        float timer;
        public YieldTime(float time)
        {
            this.time = time;
        }

        public override void Begin()
        {
            base.Begin();
            timer = 0;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            timer += Time.deltaTime;
            if (timer >= time)
            {
                state = BehaviourTreeState.SUCCESS;
            }
            else
            {
                state = BehaviourTreeState.RUNNING;
            }
            return state;
        }
    }

    public class ModifyAgentStat : Condition
    {
        readonly string stat;
        readonly object newValue;
        [Tooltip("stat can be: 1. speed 2. acceleration 3. angularSpeed")]
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
                case "angularSpeed": //new ModifyAgentStat("angularSpeed", dashAngleSpeed),

                    if (newValue is float extractedAngularSpeed)
                    {
                        brain.agent.angularSpeed = extractedAngularSpeed;
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
        readonly ExplosionEffect effect;

        public Detonate(float size, float damage, GameObject explosion) : base()
        {
            this.size = size;
            this.damage = damage;
            this.explosion = explosion;
        }

        public Detonate(float size, float damage, GameObject explosion, ExplosionEffect effect) : this(size, damage, explosion)
        {
            this.effect = effect;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            brain.GetComponent<Health>().TriggerDeath();
            GameObject boom = Object.Instantiate(explosion, brain.transform.position, Quaternion.identity, null);
            Explosion spawned = boom.GetComponent<Explosion>();
            spawned.linearScale = size;
            spawned.damage = damage;
            spawned.explosionEffect = effect;
            return BehaviourTreeState.SUCCESS;
        }
    }

    public class MoveTo : Approach
    {
        public MoveTo(string targetLocation) : base(targetLocation, 0.5f)
        {
        }
        public MoveTo(string targetLocation, PositionStoreType store) : base(targetLocation, 0.5f, store)
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
        float maxHideAngle = 45f;

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
        public TakeCover(string targetLoc, float minDist, float maxDist, float stepDist, float hideAngle) : this(targetLoc, minDist, maxDist, stepDist)
        {
            this.stepDist = Mathf.Min(stepDist, 0.97f);
            this.stepDist = Mathf.Max(this.stepDist, 0.1f);
            maxHideAngle = hideAngle;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (state == BehaviourTreeState.FAILURE)
            {
                return state;
            }
            Vector3 distance = brain.gameObject.transform.position - brain.agent.destination;
            distance.y = 0;
            if ((distance).magnitude > approachRange)
            {
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
            if (Vector3.Dot(randVect, offset) < Mathf.Cos(maxHideAngle * Mathf.Deg2Rad))
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
        readonly Type varType;
        public HasVariable(string variableName, Type variableType) : base()
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

        public InRange(string targetLocation, float distance) : base()
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

    public class EnsureInRange : Fallback
    {

        public EnsureInRange(string targetLocation, float maxRange, float approachDist)
        {
            children = new List<Node> { new InRange(targetLocation, maxRange), new Approach(targetLocation, approachDist) };
        }
    }

    public class GetOffset : Action
    {
        private readonly string target, storage;
        private readonly float distance;
        public GetOffset(string target, string storage, float distance)
        {
            this.storage = storage;
            this.target = target;
            this.distance = distance;
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (brain.memory.TryGetValue(target, out object recovered))
            {
                Vector3 targetVector = Vector3.zero;
                if (recovered is GameObject)
                {
                    targetVector = (recovered as GameObject).transform.position;
                }
                else if (recovered is Transform)
                {
                    targetVector = (recovered as Transform).position;
                }
                else if (recovered is Vector3)
                {
                    targetVector = (Vector3)recovered;
                }
                else
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
                Vector3 offset = brain.gameObject.transform.position - targetVector;
                offset = offset.normalized * distance;
                offset.y = 0;
                brain.AddOrOverwrite(storage, offset);
                state = BehaviourTreeState.SUCCESS;
                return state;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
            }
            return state;
        }
    }

    public class InvokeEvent : Action
    {
        readonly UnityEvent invoked;

        public InvokeEvent(UnityEvent invoked) : base()
        {
            this.invoked = invoked;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            invoked.Invoke();
            return BehaviourTreeState.SUCCESS;
        }
    }

    public class StoreValue : Action
    {
        readonly string current, future;
        readonly PositionStoreType from, to;
        public StoreValue(string current, string future) : base()
        {
            this.current = current;
            this.future = future;
            from = to = PositionStoreType.NULL;
        }
        public StoreValue(string current, string future, PositionStoreType from, PositionStoreType to) : this(current, future)
        {
            this.from = from;
            this.to = to;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (from == to || from == PositionStoreType.NULL || to == PositionStoreType.NULL)
                brain.AddOrOverwrite(future, brain.memory[current]);
            else if (from != PositionStoreType.VECTOR3)
            {
                if (to == PositionStoreType.GAMEOBJECT && from == PositionStoreType.TRANSFORM)
                {
                    brain.AddOrOverwrite(future, ((Transform)brain.memory[current]).gameObject);
                }
                else
                if (to == PositionStoreType.TRANSFORM && from == PositionStoreType.GAMEOBJECT)
                {
                    brain.AddOrOverwrite(future, ((GameObject)brain.memory[current]).transform);
                }
                else

                if (to == PositionStoreType.VECTOR3 && from == PositionStoreType.TRANSFORM)
                {
                    brain.AddOrOverwrite(future, ((Transform)brain.memory[current]).position);
                }
                else
                if (to == PositionStoreType.VECTOR3 && from == PositionStoreType.GAMEOBJECT)
                {
                    brain.AddOrOverwrite(future, ((GameObject)brain.memory[current]).transform.position);
                }
            }
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }

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

    public class CallVoidFunction : Action
    {
        System.Action action;

        public CallVoidFunction(System.Action func)
        {
            action = func;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            action();
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
        /// <summary>
        /// please use biggest version
        /// </summary>
        /// <param name="chargeSpeed"></param>
        /// <param name="damageZone"></param>
        /// <param name="target"></param>
        /// <param name="facingFunc"></param>
        /// <param name="resetter"></param>
        //public ChargeAttack(float chargeSpeed, GameObject damageZone, string target, System.Func<bool> facingFunc, System.Action<bool> resetter) : base()
        //{
        //    children = new List<Node> {
        //    new ModifyAgentStat("speed", 0.1f), //no speed,
        //    new ModifyAgentStat("angularSpeed", 90f), //rotate base,
        //    new RepeatUntilSuccess(new BooleanFunction(facingFunc)),
        //    new ModifyAgentStat("angularSpeed", 0f), //no rotate,
        //    new ModifyAgentStat("speed", chargeSpeed), //big speed
        //    new ModifyAgentStat("acceleration", 100f), //big speed acceleration
        //    new ToggleObject(damageZone),
        //    new SavePositionOfObject(target, "chargeTarget"),
        //    new AlwaysSucceed(new Approach("chargeTarget", 5f, PositionStoreType.VECTOR3)),
        //    new ToggleObject(damageZone),
        //    new ModifyAgentStat("angularSpeed", 30f), //reset vals
        //    new ModifyAgentStat("speed", 3.5f),
        //    new ModifyAgentStat("acceleration", 8f), //big speed
        //    new CallVoidFunctionWithBool(resetter, true)
        //    };
        //}
        //public ChargeAttack(float chargeSpeed, GameObject damageZone, string target, System.Func<string, bool> facingFunc, System.Action<bool> resetter) : base()
        //{
        //    children = new List<Node> {
        //    new ModifyAgentStat("speed", 0.1f), //no speed,
        //    new ModifyAgentStat("angularSpeed", 90f), //rotate base,
        //    new RepeatUntilSuccess(new BooleanFunction(facingFunc, target)),
        //    new ModifyAgentStat("angularSpeed", 0f), //no rotate,
        //    new ModifyAgentStat("speed", chargeSpeed), //big speed
        //    new ModifyAgentStat("acceleration", 100f), //big speed acceleration
        //    new ToggleObject(damageZone),
        //    new SavePositionOfObject(target, "chargeTarget"),
        //    new AlwaysSucceed(new Approach("chargeTarget", 5f, PositionStoreType.VECTOR3)),
        //    new ToggleObject(damageZone),
        //    new ModifyAgentStat("angularSpeed", 30f), //reset vals
        //    new ModifyAgentStat("speed", 3.5f),
        //    new ModifyAgentStat("acceleration", 8f), //big speed
        //    new CallVoidFunctionWithBool(resetter, true)
        //    };
        //}
        //public ChargeAttack(float chargeSpeed, GameObject damageZone, string target, System.Func<string, bool> facingFunc, System.Action<bool> resetter, System.Action<bool> overrideToggle) : base()
        //{
        //    children = new List<Node> {
        //    new CallVoidFunctionWithBool(overrideToggle, true),
        //    new ModifyAgentStat("speed", 0.1f), //no speed,
        //    new ModifyAgentStat("angularSpeed", 90f), //rotate base,
        //    new RepeatUntilSuccess(new BooleanFunction(facingFunc, target)),
        //    new ModifyAgentStat("angularSpeed", 0f), //no rotate,
        //    new ModifyAgentStat("speed", chargeSpeed), //big speed
        //    new ModifyAgentStat("acceleration", 200f), //big speed acceleration
        //    new ToggleObject(damageZone),
        //    new SavePositionOfObject(target, "chargeTarget"),
        //    new AlwaysSucceed(new Approach("chargeTarget", 5f, PositionStoreType.VECTOR3)),
        //    new ToggleObject(damageZone),
        //    new ModifyAgentStat("angularSpeed", 30f), //reset vals
        //    new ModifyAgentStat("speed", 3.5f),
        //    new ModifyAgentStat("acceleration", 8f), //big speed
        //    new CallVoidFunctionWithBool(overrideToggle, false),
        //    new CallVoidFunctionWithBool(resetter, true)
        //    };
        //}
        public ChargeAttack(float chargeSpeed, float chargeAcceleration, float aimingRotationSpeed, GameObject damageZone, string target, System.Func<string, bool> facingFunc,
            System.Action<bool> resetter, System.Action<bool> overrideToggle, System.Action<bool> lookToggle, bool look, UnityEvent prep,
            UnityEvent start, UnityEvent end) : base()
        {
            children = new List<Node> {
                new InvokeEvent(prep),
            new CallVoidFunctionWithBool(overrideToggle, true),
            new ModifyAgentStat("speed", 0f), //no speed,
            new ModifyAgentStat("angularSpeed", aimingRotationSpeed), //rotate base,
            new ModifyAgentStat("acceleration", 9999f),
            new RepeatUntilSuccess(new BooleanFunction(facingFunc, target)),
            new CallVoidFunctionWithBool(lookToggle, look),
                new InvokeEvent(start),
            new ModifyAgentStat("angularSpeed", 0f), //no rotate,
            new ModifyAgentStat("speed", chargeSpeed), //big speed
            new ModifyAgentStat("acceleration", chargeAcceleration), //big speed acceleration   
            new ToggleObject(damageZone),
            new FindChargeTarget(target, "chargeTarget"),
            new AlwaysSucceed(new Approach("chargeTarget", 0.1f, PositionStoreType.VECTOR3)),
            new ToggleObject(damageZone),
            new ModifyAgentStat("angularSpeed", 30f), //reset vals
            new ModifyAgentStat("speed", 3.5f),
            new ModifyAgentStat("acceleration", 8f), //big speed
            new CallVoidFunctionWithBool(lookToggle, false),
            new CallVoidFunctionWithBool(overrideToggle, false),
            new CallVoidFunctionWithBool(resetter, true),
                new InvokeEvent(end)
            };
        }
    }

    public class Wander : Action
    {
        Vector3 direction = Vector3.zero;
        float maxStepDistance;
        float varianceFactor;
        string locationStore;

        public Wander() : base()
        {
            locationStore = "uniqueStorageLocationWander";
            children.Add(new Approach(locationStore, 0.01f, PositionStoreType.VECTOR3));
        }
        /// <summary>
        /// Move a small step in a random direction
        /// </summary>
        /// <param name="locationStore">Unique string to carry target position around</param>
        /// <param name="variance">A measure of how much the last step influences new direction (0 = no change, 1 = use new fully)</param>
        /// <param name="maxStepDistance"></param>
        public Wander(string locationStore, float variance, float maxStepDistance) : base()
        {
            this.locationStore = locationStore;
            children.Add(new Approach(locationStore, 0.01f, PositionStoreType.VECTOR3));
            varianceFactor = variance;
            this.maxStepDistance = maxStepDistance;
        }
        public Wander(string locationStore, float variance, float maxStepDistance, float approachDist) : base()
        {
            this.locationStore = locationStore;
            children.Add(new Approach(locationStore, approachDist, PositionStoreType.VECTOR3));
            varianceFactor = variance;
            this.maxStepDistance = maxStepDistance;
        }

        public override void Begin()
        {
            base.Begin();
            Vector3 newDirection = Random.insideUnitCircle;
            newDirection.z = newDirection.y;
            newDirection.y = 0;
            direction = Vector3.Lerp(direction, newDirection, varianceFactor);
            brain.AddOrOverwrite(locationStore, brain.gameObject.transform.position + direction * maxStepDistance);
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            state = children[0].Tick();
            BehaviourTreeState returnState = state;
            if (state != BehaviourTreeState.RUNNING)
            {
                Restart();
            }
            return returnState;
        }
    }


    public class FindChargeTarget : Action
    {
        string obj, sto;

        public FindChargeTarget(string objectRef, string positionStorage)
        {
            this.obj = objectRef;
            this.sto = positionStorage;
        }

        public override BehaviourTreeState Tick()
        {
            Vector3 objectPos;
            if (brain.memory.TryGetValue(obj, out object o))
            {
                if (o is Transform)
                {
                    objectPos = (o as Transform).position;
                    //brain.AddOrOverwrite(sto, (o as Transform).position);
                }
                else if (o is GameObject)
                {
                    objectPos = (o as GameObject).transform.position;
                    //brain.AddOrOverwrite(sto, (o as GameObject).transform.position);
                }
                else
                {
                    state = BehaviourTreeState.FAILURE;
                    return state;
                }
                Vector3 direction = objectPos - brain.gameObject.transform.position;
                direction = direction.normalized;
                Vector3 pathTarget = objectPos;
                float stepSize = 0.1f;
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(brain.gameObject.transform.position, pathTarget, NavMesh.AllAreas, path);
                while (path.status == NavMeshPathStatus.PathComplete)
                {
                    pathTarget += direction * stepSize;
                    NavMesh.CalculatePath(brain.gameObject.transform.position, pathTarget, NavMesh.AllAreas, path);
                }
                brain.AddOrOverwrite(sto, pathTarget);
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

    public class HasLineOfSight : Condition
    {
        readonly string target;
        readonly PositionStoreType storeType;

        public HasLineOfSight(string target, PositionStoreType storeType)
        {
            this.storeType = storeType;
            this.target = target;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (brain.memory.TryGetValue(target, out object recovered))
            {
                Vector3 targetPosition = Vector3.zero;
                switch (storeType)
                {
                    case PositionStoreType.NULL:
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    case PositionStoreType.VECTOR3:
                        targetPosition = (Vector3)recovered;
                        break;
                    case PositionStoreType.TRANSFORM:
                        targetPosition = (recovered as Transform).position;
                        break;
                    case PositionStoreType.GAMEOBJECT:
                        targetPosition = (recovered as GameObject).transform.position;
                        break;
                }
                if (Physics.Linecast(brain.transform.position + Vector3.up, targetPosition + Vector3.up, out RaycastHit hit, ~LayerMask.GetMask(LayerMask.LayerToName(6))))
                    state = BehaviourTreeState.FAILURE;
                else
                    state = BehaviourTreeState.SUCCESS;
                if (brain.debug)
                {
                    Debug.Log(state);
                    Debug.Log(hit.collider);
                }
                return state;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
        }
    }


    public class FindLineOfSightPosition : Action
    {
        readonly string storeLocation;
        readonly int stepCount;
        readonly float stepDist;
        readonly string target;
        readonly PositionStoreType storeType;
        public FindLineOfSightPosition(string storeLocation)
        {
            this.storeLocation = storeLocation;
        }

        public FindLineOfSightPosition(string storeLocation, int stepCount, float stepDist) : this(storeLocation)
        {
            this.stepCount = stepCount;
            this.stepDist = stepDist;
        }

        public FindLineOfSightPosition(string storeLocation, int stepCount, float stepDist, string target, PositionStoreType storeType) : this(storeLocation, stepCount, stepDist)
        {
            this.target = target;
            this.storeType = storeType;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (brain.memory.TryGetValue(target, out object recovered))
            {
                Vector3 targetPosition = Vector3.zero;
                switch (storeType)
                {
                    case PositionStoreType.NULL:
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    case PositionStoreType.VECTOR3:
                        targetPosition = (Vector3)recovered;
                        break;
                    case PositionStoreType.TRANSFORM:
                        targetPosition = (recovered as Transform).position;
                        break;
                    case PositionStoreType.GAMEOBJECT:
                        targetPosition = (recovered as GameObject).transform.position;
                        break;
                }
                Vector3 toTarget = targetPosition - brain.transform.position;
                Vector3 left = Vector3.Cross(toTarget, Vector3.up);
                left = left.normalized * stepDist;
                targetPosition += Vector3.up;
                Vector3 offsetBrain = brain.transform.position + Vector3.up;
                state = BehaviourTreeState.FAILURE;
                for (int i = 1; i <= stepCount; i++)
                {
                    Vector3 testPosition = offsetBrain + (left * i);
                    if (!Physics.Linecast(testPosition, targetPosition, ~LayerMask.GetMask(LayerMask.LayerToName(6))))
                    {
                        brain.AddOrOverwrite(storeLocation, testPosition);
                        state = BehaviourTreeState.SUCCESS;
                        break;
                    }
                    testPosition = offsetBrain - (left * i);
                    if (!Physics.Linecast(testPosition, targetPosition, ~LayerMask.GetMask(LayerMask.LayerToName(6))))
                    {
                        brain.AddOrOverwrite(storeLocation, testPosition);
                        state = BehaviourTreeState.SUCCESS;
                        break;
                    }
                }
                return state;
            }
            else
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
        }
    }

    public class ApproachUntilLineOfSight : Action
    {
        readonly string targetLocation;
        readonly float approachDist;
        Vector3 activeTarget;
        GameObject objectTarget;
        Transform transformTarget;
        readonly PositionStoreType store;
        public ApproachUntilLineOfSight(string targetLocation, float approachDist) : base()
        {
            this.targetLocation = targetLocation;
            this.approachDist = approachDist;
            store = PositionStoreType.GAMEOBJECT;
        }

        public ApproachUntilLineOfSight(string targetLocation, float approachDist, PositionStoreType type) : this(targetLocation, approachDist)
        {
            store = type;
        }

        public override void Begin()
        {
            base.Begin();
            state = BehaviourTreeState.RUNNING;
            if (!brain.memory.TryGetValue(targetLocation, out object recovered))
            {
                state = BehaviourTreeState.FAILURE;
            }

            switch (store)
            {
                case PositionStoreType.NULL:
                    activeTarget = new Vector3(0, 0, 0);
                    state = BehaviourTreeState.FAILURE;
                    break;
                case PositionStoreType.GAMEOBJECT:
                    objectTarget = (GameObject)recovered;
                    activeTarget = objectTarget.transform.position;
                    break;
                case PositionStoreType.VECTOR3:
                    activeTarget = (Vector3)recovered;
                    break;
                case PositionStoreType.TRANSFORM:
                    transformTarget = (Transform)recovered;
                    activeTarget = transformTarget.position;
                    break;
                default:
                    activeTarget = brain.transform.position;
                    break;
            }
            brain.agent.SetDestination(activeTarget);
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            switch (store)
            {
                case PositionStoreType.NULL:
                    break;
                case PositionStoreType.GAMEOBJECT:
                    if (objectTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        //Hvae I succeeded, or failed?
                        /*
                         If I want to stay near something until they die I have succeded if they die
                         If my follow up to strafe in range is like, kiss the target, then failure might be more appropriate, because target is gone

                         If I "StrafeInRange until failure" then loss of target being a failure is a good idea
                         */

                        return state;
                    }
                    activeTarget = objectTarget.transform.position;
                    brain.agent.SetDestination(activeTarget);
                    break;
                case PositionStoreType.VECTOR3:
                    if (brain.memory.TryGetValue(targetLocation, out object recovered))
                    {
                        activeTarget = (Vector3)recovered;
                    }
                    else
                    {
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    }
                    break;
                case PositionStoreType.TRANSFORM:
                    if (transformTarget == null)
                    {
                        state = BehaviourTreeState.FAILURE;
                        return state;
                    }
                    activeTarget = transformTarget.position;
                    break;
                default:
                    state = BehaviourTreeState.FAILURE;
                    return state;
            }
            Vector3 testPosition = brain.transform.position + Vector3.up;

            if (!Physics.Linecast(testPosition, activeTarget + Vector3.up, ~LayerMask.GetMask(LayerMask.LayerToName(6))))
            {
                state = BehaviourTreeState.SUCCESS;
                brain.agent.SetDestination(brain.transform.position);
            }
            return state;
        }
    }

    //public class GetDVDBouncePoint : Action
    //{
    //    string velocityData;
    //    string destinationStorage;
    //    Vector3 velocity;
    //    float sideStepDist = 0.5f;

    //    public GetDVDBouncePoint(string velocityData, string destinationStorage)
    //    {
    //        this.velocityData = velocityData;
    //        this.destinationStorage = destinationStorage;
    //    }

    //    public override void Begin()
    //    {
    //        base.Begin();
    //        state = BehaviourTreeState.RUNNING;
    //        if(brain.memory.TryGetValue(velocityData, out object extracted))
    //        {
    //            if(extracted is Vector3)
    //            {
    //                velocity = (Vector3)extracted;
    //            }
    //            else
    //            {
    //                state = BehaviourTreeState.FAILURE;
    //                return;
    //            }
    //            Vector3 pos = brain.gameObject.transform.position;
    //            pos.y += 0.25f; //high enough not to collide with the ground, low enough to see walls before holes in the floor
    //            Physics.Raycast(pos, velocity, out RaycastHit hitInfo, 100f, ~LayerMask.GetMask(LayerMask.LayerToName(0)));
    //            Vector3 sideStep = Vector3.Cross(velocity, Vector3.up);
    //            sideStep = sideStep.normalized * sideStepDist;
    //            sideStep.y = 0.5f;
    //            NavMeshPath probePath = new NavMeshPath();
    //            NavMeshPath leftSideStep = new NavMeshPath();//might be right actually
    //            NavMeshPath rightSideStep = new NavMeshPath();//might be left actually
    //            Vector3 hitPoint = hitInfo.point;
    //            hitPoint.y = 0;
    //            //Debug.Log(brain.agent.CalculatePath(hitPoint, probePath));
    //            //Debug.Log(NavMesh.CalculatePath(brain.gameObject.transform.position, hitPoint, 7, probePath));
    //            Debug.Log(NavMesh.FindClosestEdge(hitPoint, out NavMeshHit navMeshHit, -1));
    //            Debug.Log(brain.agent.CalculatePath(hitPoint + sideStep, leftSideStep));
    //            Debug.Log(brain.agent.CalculatePath(hitPoint - sideStep, rightSideStep));

    //            Vector3 probeRes, leftRes, rightRes;
    //            probeRes = probePath.corners[probePath.corners.Length - 1];
    //            leftRes = leftSideStep.corners[leftSideStep.corners.Length - 1];
    //            rightRes = rightSideStep.corners[rightSideStep.corners.Length - 1];
    //            probeRes.y = 0;
    //            leftRes.y = 0;
    //            rightRes.y = 0;
    //            Vector3 lp = probeRes - leftRes;
    //            Vector3 rp = rightRes - probeRes;
    //            if((lp - rp).sqrMagnitude < 0.01f)
    //            {
    //                Debug.Log("CASE CO-LINEAR");
    //                float y1 = rightRes.z;
    //                float y2 = leftRes.z;
    //                float x1 = rightRes.x;
    //                float x2 = leftRes.x;

    //                float c = ((y1 / x1 - y2 / x2) / (1f / x1 - 1f / x2));

    //                float m = (y1 - c) / x1;

    //                float m2 = velocity.z / velocity.x;

    //                float xt = c / (m2 - m);

    //                float yt = xt * m2;

    //                Vector3 destination = new Vector3(xt, 0, yt);

    //                Vector3 normal = hitInfo.normal.normalized;

    //                Vector3 newVel = velocity - 2*(Vector3.Dot(velocity, normal)) * normal;
    //                Debug.Log(c);
    //                brain.AddOrOverwrite(destinationStorage, destination);
    //                brain.AddOrOverwrite(velocityData, newVel);
    //                state = BehaviourTreeState.SUCCESS;
    //                //lr, pr, and rr are co-linear
    //                //thus, the line lr-rr should intersect at the point on the edge we seek
    //            }
    //            else
    //            {
    //                if(lp.magnitude < 0.01f && rp.magnitude < 0.01f)
    //                {
    //                    //corner
    //                    Debug.Log("CASE CO-ORNER");
    //                    brain.AddOrOverwrite(destinationStorage, probeRes);
    //                    brain.AddOrOverwrite(velocityData, -velocity);
    //                    state = BehaviourTreeState.SUCCESS;
    //                }
    //                else
    //                {
    //                    if(lp.magnitude < rp.magnitude)
    //                    {

    //                        Debug.Log("CASE USE LEFT");
    //                        float y1 = probeRes.z;
    //                        float y2 = leftRes.z;
    //                        float x1 = probeRes.x;
    //                        float x2 = leftRes.x;

    //                        float c = ((y1 / x1 - y2 / x2) / (1 / x1 - 1 / x2));

    //                        float m = (y1 - c) / x1;

    //                        float m2 = velocity.z / velocity.x;

    //                        float xt = c / (m2 - m);

    //                        float yt = xt * m2;

    //                        Vector3 destination = new Vector3(xt, 0, yt);

    //                        Vector3 normal = hitInfo.normal.normalized;

    //                        Vector3 newVel = velocity - 2 * (Vector3.Dot(velocity, normal)) * normal;

    //                        brain.AddOrOverwrite(destinationStorage, destination);
    //                        brain.AddOrOverwrite(velocityData, newVel);
    //                        state = BehaviourTreeState.SUCCESS;
    //                    }
    //                    else
    //                    {

    //                        Debug.Log("CASE USE RIGHT");
    //                        float y1 = rightRes.z;
    //                        float y2 = probeRes.z;
    //                        float x1 = rightRes.x;
    //                        float x2 = probeRes.x;

    //                        float c = ((y1 / x1 - y2 / x2) / (1 / x1 - 1 / x2));

    //                        float m = (y1 - c) / x1;

    //                        float m2 = velocity.z / velocity.x;

    //                        float xt = c / (m2 - m);

    //                        float yt = xt * m2;

    //                        Vector3 destination = new Vector3(xt, 0, yt);

    //                        Vector3 normal = hitInfo.normal.normalized;

    //                        Vector3 newVel = velocity - 2 * (Vector3.Dot(velocity, normal)) * normal;

    //                        brain.AddOrOverwrite(destinationStorage, destination);
    //                        brain.AddOrOverwrite(velocityData, newVel);
    //                        state = BehaviourTreeState.SUCCESS;
    //                    }
    //                }
    //                //check for corner, then use the smaller of lp/rp if not corner
    //            }
    //        }
    //        else
    //        {
    //            state = BehaviourTreeState.FAILURE;
    //        }
    //    }

    //    public override BehaviourTreeState Tick()
    //    {
    //        base.Tick();
    //        return state;
    //    }
    //}
    #endregion
}