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

    public enum StoreType
    {
        NULL,
        POSITION,
        TRANSFORM,
        GAMEOBJECT
    }

    public abstract class BehaviourTree : Health, IShortCircuitable
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
        internal bool isShieldable = true;

        public RootNode replacement;
        CharacterVFXManager VFXManager;
        EnemyDamageNumberSpawner numbers;
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

        internal override void Awake()
        {
            base.Awake();
            numbers = GetComponentInChildren<EnemyDamageNumberSpawner>();
            VFXManager = GetComponentInChildren<CharacterVFXManager>();
            player = GameObject.FindGameObjectWithTag("Player");
            agent = GetComponent<NavMeshAgent>();
            memory = new Dictionary<string, object>();
            meshesRef = GetComponentInChildren<MeshFilter>().gameObject;
        }
        public virtual void FixedUpdate()
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

        internal override void TriggerDeath()
        {
            base.TriggerDeath();
            TriggerDebrisExplosion tde = GetComponentInChildren<TriggerDebrisExplosion>();
            if (tde != null)
            {
                tde.TriggerExplosion();
            }
            Collider[] c = GetComponents<Collider>();
            if (c.Length > 0)
            {
                foreach (Collider x in c)
                {
                    x.enabled = false;
                }
            }
            gameObject.tag = "Untagged";
            EnemyGun[] guns = GetComponentsInChildren<EnemyGun>();
            foreach (EnemyGun g in guns)
            {
                g.BeGone();
            }
            foreach (Transform child in transform)
            {
                child.parent = null;
            }
            Die();
            Destroy(gameObject);
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
            if (verboseDebug || debug)
            {
                Debug.Log(string.Format("{0} should not move.", name));
            }
        }

        public virtual void Resume()
        {
            if (agent != null)
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

        public virtual void Die()
        {
            Stop();
            isShieldable = false;
        }

        public virtual void ShortCircuit(float chance, float time)
        {
            if (chance >= UnityEngine.Random.Range(0, 100))
            {
                if (VFXManager != null)
                {
                    VFXManager.ToggleEffectVFX(effect.ShortCircuit, true);
                }
                StopForTime(time);
                //apply VFX for time
            }
        }

        internal override float TakeDamage(float amount, bool isCrit = false)
        {
            float moddedAmount = base.TakeDamage(amount, isCrit);
            if(numbers!=null)
            numbers.SpawnDamageNumber(moddedAmount, transform.position, isCrit);
            return moddedAmount;
        }


        public override void Burn(float chance, float damageTick, int tickCount)
        {
            //Debug.Log("Burn start");
            base.Burn(chance, damageTick, tickCount);
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
        internal float Weight;
        public virtual float weight
        {
            get
            {
                return Weight;
            }
            private set
            {
                Weight = value;
            }
        }
        public WeightedRandomChoice(Node child) : base(child)
        {
        }

        public WeightedRandomChoice(Node child, float weight) : this(child)
        {
            this.weight = weight;
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
        Func<float> calculate;
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

        public override float weight
        {
            get
            {
                if (calculate != null)
                    Weight = calculate();
                return Weight;
            }
        }
    }

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
                float childWeight = w.weight;
                if (w.Weight > 0.01f && (allowRepeat || children.IndexOf(w) != lastIndex))
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
                    Debug.LogWarning("Boss Move Selector Failed");
                    childIndex = 0;
                    return;
                }
                else
                {
                    allowRepeat = true;
                    weightTotal = 0f;
                    foreach (WeightedRandomChoice w in children)
                    {
                        float childWeight = w.weight;
                        if (w.Weight > 0.01f)
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
                        Debug.LogWarning("Boss Move Selector Failed (even tried to repeat)");
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

        public Parallel(int threshold, params Node[] children): this(children)
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
            foreach(Node n in children)
            {
                if(n.state == BehaviourTreeState.RUNNING || n.state == BehaviourTreeState.NULL)
                {
                    n.Tick();
                    if(n.state == BehaviourTreeState.SUCCESS)
                    {
                        successCount++;
                        completeCount++;
                    }
                    else if(n.state == BehaviourTreeState.FAILURE)
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
    //Parallel?

    public class MultiRoot : Control
    {
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
            foreach(Node n in children)
            {
                if(n.Tick()!=BehaviourTreeState.RUNNING)
                {
                    n.Restart();
                }
            }
            return BehaviourTreeState.RUNNING;
        }
    }

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

    public class FindTarget : Action
    {
        string targetType;
        string storage;

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

        readonly StoreType type;

        public StrafeInRange(string targetLocation, float minDist, float maxDist) : base()
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            targLoc = targetLocation;
            type = StoreType.GAMEOBJECT;
        }
        public StrafeInRange(string targetLocation, float minDist, float maxDist, StoreType type) : this(targetLocation, minDist, maxDist)
        {
            this.minDist = minDist;
            this.maxDist = maxDist;
            targLoc = targetLocation;
            this.type = type;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            object recovered;
            if (!brain.memory.TryGetValue(targLoc, out recovered))
            {
                state = BehaviourTreeState.FAILURE;
                return state;
            }
            switch (type)
            {
                case StoreType.NULL:
                    if (brain.debug)
                    {
                        Debug.Log("You had to put in effirt to end up here");
                    }
                    state = BehaviourTreeState.FAILURE;
                    return state;
                case StoreType.GAMEOBJECT:
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
                case StoreType.POSITION:
                    target = (Vector3)recovered;
                    break;
                case StoreType.TRANSFORM:
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
                case StoreType.NULL:
                    target = new Vector3(0, 0, 0);
                    break;
                case StoreType.GAMEOBJECT:
                    gameObjectTarget = (GameObject)recovered;
                    target = gameObjectTarget.transform.position;
                    break;
                case StoreType.POSITION:
                    target = (Vector3)recovered;
                    break;
                case StoreType.TRANSFORM:
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
        StoreType store;
        public Approach(string targetLocation, float approachDist) : base()
        {
            this.targetLocation = targetLocation;
            this.approachDist = approachDist;
            store = StoreType.GAMEOBJECT;
        }

        public Approach(string targetLocation, float approachDist, StoreType type) : this(targetLocation, approachDist)
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
                case StoreType.NULL:
                    activeTarget = new Vector3(0, 0, 0);
                    state = BehaviourTreeState.FAILURE;
                    break;
                case StoreType.GAMEOBJECT:
                    objectTarget = (GameObject)recovered;
                    activeTarget = objectTarget.transform.position;
                    break;
                case StoreType.POSITION:
                    activeTarget = (Vector3)recovered;
                    break;
                case StoreType.TRANSFORM:
                    transformTarget = (Transform)recovered;
                    activeTarget = transformTarget.position;
                    break;
                default:
                    activeTarget = new Vector3(0, 0, 0);
                    break;
            }
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
                case StoreType.NULL:
                    break;
                case StoreType.GAMEOBJECT:
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
                    break;
                case StoreType.POSITION:
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
                case StoreType.TRANSFORM:
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

            brain.agent.SetDestination(activeTarget);
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
        public MoveTo(string targetLocation, StoreType store) : base(targetLocation, 0.5f, store)
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

    public class InvokeEvent : Action
    {
        UnityEvent invoked;

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
        readonly StoreType from, to;
        public StoreValue(string current, string future) : base()
        {
            this.current = current;
            this.future = future;
            from = to = StoreType.NULL;
        }
        public StoreValue(string current, string future, StoreType from, StoreType to) : this(current, future)
        {
            this.from = from;
            this.to = to;
        }

        public override BehaviourTreeState Tick()
        {
            base.Tick();
            if (from == to || from == StoreType.NULL || to == StoreType.NULL)
                brain.AddOrOverwrite(future, brain.memory[current]);
            else if (from != StoreType.POSITION)
            {
                if (to == StoreType.GAMEOBJECT && from == StoreType.TRANSFORM)
                {
                    brain.AddOrOverwrite(future, ((Transform)brain.memory[current]).gameObject);
                }
                else
                if (to == StoreType.TRANSFORM && from == StoreType.GAMEOBJECT)
                {
                    brain.AddOrOverwrite(future, ((GameObject)brain.memory[current]).transform);
                }
                else

                if (to == StoreType.POSITION && from == StoreType.TRANSFORM)
                {
                    brain.AddOrOverwrite(future, ((Transform)brain.memory[current]).position);
                }
                else
                if (to == StoreType.POSITION && from == StoreType.GAMEOBJECT)
                {
                    brain.AddOrOverwrite(future, ((GameObject)brain.memory[current]).transform.position);
                }
            }
            state = BehaviourTreeState.SUCCESS;
            return state;
        }
    }
}