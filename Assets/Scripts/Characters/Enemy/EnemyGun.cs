using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public GameObject gunPoint;
    public GameObject shotPattern;
    public float minDelay, maxDelay;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireOnRepeat());
    }

    IEnumerator FireOnRepeat()
    {
        float timer = 0f;
        float randTime;
        while(true)
        {
            randTime = Random.Range(minDelay, maxDelay);
            while(timer < randTime)
            {
                yield return null;
                timer += Time.deltaTime;
            }
            Instantiate(shotPattern, gunPoint.transform.position, gunPoint.transform.rotation, null);
            anim.SetTrigger("shoot");
            timer = 0f;
        }
    }

    public void BeGone()
    {
        StopAllCoroutines();
    }
}
