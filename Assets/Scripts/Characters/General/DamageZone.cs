using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float entryDamage;

    public bool continuous = false;
    public float dotDamage;
    public float dotTime = 1f;

    private Dictionary<Health, Coroutine> dots;
    // Start is called before the first frame update
    void Start()
    {
        dots = new Dictionary<Health, Coroutine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health hp))
        {
            hp.TakeDamage(entryDamage);
            if (continuous && !dots.ContainsKey(hp))
            {
                Coroutine c = StartCoroutine(DamageOverTime(hp));
                dots.Add(hp, c);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Health hp))
        {
            if (continuous && dots.ContainsKey(hp))
            {
                StopCoroutine(dots[hp]);
                dots.Remove(hp);
            }
        }
    }

    IEnumerator DamageOverTime(Health target)
    {
        float timer = 0f;
        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            if(timer >= dotTime)
            {
                target.TakeDamage(dotDamage);
                timer -= dotTime;
            }
        }
    }
}
