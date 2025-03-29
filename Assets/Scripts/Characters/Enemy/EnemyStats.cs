using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _baseSpeed;
    [SerializeField] float _baseAngularSpeed;
    [SerializeField] float _baseAcceleration;
    [SerializeField] float _difficulty = 1f;
    [SerializeField] float _baseDamage = 25f;//default bullet damage

    public UnityEvent onStatsChanged;

    [SerializeField] Health myHealth;
    [SerializeField] NavMeshAgent myAgent;

    public virtual float damage
    {
        get
        {
            return _baseDamage * (1f + (difficulty - 1f) * 0.25f);
        }
    }

    public virtual float difficulty
    {
        get
        {
            return _difficulty;
        }
        set
        {
            if (_difficulty != value)
            {
                _difficulty = value;
                onStatsChanged.Invoke();
            }
        }
    }

    public virtual float maxHealth
    {
        get
        {
            return _baseMaxHealth * (1f + (difficulty - 1f) * 0.5f);
        }
        private set
        {
            _baseMaxHealth = value;
        }
    }

    public virtual float speed
    {
        get
        {
            return _baseSpeed;
        }
    }

    public virtual float angularSpeed
    {
        get
        {
            return _baseAngularSpeed;
        }
    }

    public virtual float acceleration
    {
        get
        {
            return _baseAcceleration;
        }
    }

    HealthStats _healthStats;
    NavAgentStats _navAgentStats;

    public virtual HealthStats healthStats
    {
        get
        {
            if (_healthStats.max != maxHealth)
                _healthStats = new HealthStats(maxHealth);
            return _healthStats;
        }
        set
        {
            _healthStats = value;
        }
    }

    public struct HealthStats
    {
        public float max;
        public float current;

        public HealthStats(float max)
        {
            this.max = max;
            current = max;
        }
    }

    public virtual NavAgentStats navAgentStats
    {
        get
        {
            if (_navAgentStats.speed == 0)
            {
                _navAgentStats = new NavAgentStats(speed, angularSpeed, acceleration);
            }
            return _navAgentStats;
        }
        set
        {
            _navAgentStats = value;
        }
    }

    public struct NavAgentStats
    {
        public float speed, angularSpeed, acceleration;

        public NavAgentStats(float speed, float angularSpeed, float acceleration)
        {
            this.speed = speed;
            this.angularSpeed = angularSpeed;
            this.acceleration = acceleration;
        }
    }

    private void OnEnable()
    {
        onStatsChanged.AddListener(UpdateStats);
    }

    private void OnDisable()
    {
        onStatsChanged.RemoveListener(UpdateStats);
    }

    // Start is called before the first frame update
    internal virtual void Start()
    {
        UpdateStats();
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal virtual void UpdateStats()
    {
        if (myAgent == null)
        {
            if (TryGetComponent<NavMeshAgent>(out myAgent))
            {
                myAgent.speed = navAgentStats.speed;
                myAgent.acceleration = navAgentStats.acceleration;
                myAgent.angularSpeed = navAgentStats.angularSpeed;
            }
        }
        else
        {
            myAgent.speed = navAgentStats.speed;
            myAgent.acceleration = navAgentStats.acceleration;
            myAgent.angularSpeed = navAgentStats.angularSpeed;
        }
        if (myHealth == null)
        {
            if (TryGetComponent<Health>(out myHealth))
            {
                myHealth.maxHealth = healthStats.max;
                myHealth.health = healthStats.current;
            }
        }
        else
        {
            myAgent.speed = navAgentStats.speed;
            myAgent.acceleration = navAgentStats.acceleration;
            myAgent.angularSpeed = navAgentStats.angularSpeed;
        }
    }
}
