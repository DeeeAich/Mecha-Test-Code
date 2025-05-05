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

    internal override IEnumerator DamageOverTime(Health target)
    {
        float timer = 0f;
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer >= dotTime)
            {
                teslaVFX.ZapObject(target.gameObject);//Make Toms lightning script go off
                target.TakeDamage(dotDamage);
                timer -= dotTime;
            }
        }
    }

    internal override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health hp))
        {
            hp.TakeDamage(entryDamage);
            if (continuous && !dots.ContainsKey(hp))
            {
                teslaVFX.ZapObject(hp.gameObject);//Make Toms lightning script go off
                Coroutine c = StartCoroutine(DamageOverTime(hp));
                dots.Add(hp, c);
            }
        }
    }
}
