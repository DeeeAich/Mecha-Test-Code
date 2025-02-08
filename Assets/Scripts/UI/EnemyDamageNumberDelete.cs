using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageNumberDelete : MonoBehaviour
{
    

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
