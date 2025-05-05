using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaDamageZone : DamageZone
{
    [SerializeField] private TeslaVFX teslaVFX;

    private void Awake()
    {
        teslaVFX = transform.parent.GetComponentInChildren<TeslaVFX>();
    }

    internal override IEnumerator DamageOverTime(Health target, ZoneDamageOverTimeInfo info)
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

    internal override void OnTriggerEnter(Collider other)
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
}
