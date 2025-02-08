using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageNumberDelete : MonoBehaviour
{

    [SerializeField] float deathTimer = 0.75f;

    void Start()
    {
        StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.75f);
        if (EnemyDamageNumberSpawner.instance.damageNumbers.Contains(gameObject))
        {
            EnemyDamageNumberSpawner.instance.damageNumbers.Remove(gameObject);
            DestroyImmediate(gameObject);
        }
    }

}
