using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyStats : MonoBehaviour
{
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _difficulty;

    public UnityEvent onStatsChanged;

    public float difficulty
    {
        get
        {
            return _difficulty;
        }
        set
        {
            if(_difficulty != value)
            {
                _difficulty = value;
                onStatsChanged.Invoke();
            }    
        }
    }

    public float maxHealth
    {
        get
        {
            return _baseMaxHealth * difficulty;
        }
        private set
        {
            _baseMaxHealth = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
