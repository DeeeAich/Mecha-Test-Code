using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float entryDamage;

    public bool continuous = false;
    public float dotDamage;
    public float dotTime = 1f;

    public class ZoneDamageOverTimeInfo
    {
        public bool inside;
        public Coroutine coroutine;
    }

    internal Dictionary<Health, ZoneDamageOverTimeInfo> dots;
    // Start is called before the first frame update
    internal void Start()
    {
        dots = new Dictionary<Health, ZoneDamageOverTimeInfo>();
    }

    internal virtual void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health hp))
        {
            if (continuous && !dots.ContainsKey(hp))
            {
                hp.TakeDamage(entryDamage);
                ZoneDamageOverTimeInfo newInfo = new ZoneDamageOverTimeInfo();
                newInfo.inside = true;
                newInfo.coroutine = StartCoroutine(DamageOverTime(hp, newInfo));
            }
            else if (continuous)
            {
                dots[hp].inside = true;
            }
            else
            {
                hp.TakeDamage(entryDamage);
            }
        }
    }

    internal void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Health hp))
        {
            if (continuous && dots.ContainsKey(hp))
            {
                dots[hp].inside = false;
            }
        }
    }

    internal virtual IEnumerator DamageOverTime(Health target, ZoneDamageOverTimeInfo info)
    {
        float timer = 0f;
        while (true)
        {
            yield return null;
            if (info.inside)
                timer += Time.deltaTime;
            if (timer >= dotTime && target != null)
            {
                target.TakeDamage(dotDamage);
                timer -= dotTime;
            }
        }
    }
}
