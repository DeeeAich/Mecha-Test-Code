using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerNailGun : MonoBehaviour
{
    public Animator anim;
    public GameObject gunPoint;
    public GameObject shotPattern;

    public float startDelayMin, startDelayMax;
    public float minDelay, maxDelay, reloadTime;
    public int magSize = 10;
    public bool pause = false;
    private int magCount;
    // Start is called before the first frame update
    void Start()
    {
        magCount = magSize;



        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    either:
        1. This is performing a firing loop (with a reload when empty)
        2. This outsources the loop to elsewhere but provides the means to fire
    */

    IEnumerator StartDelay()
    {
        float timer = 0f;
        float randTime = Random.Range(startDelayMin, startDelayMax);
        while (timer < randTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        StartCoroutine(FireOnRepeat());
    }

    IEnumerator FireOnRepeat()
    {
        float timer = 0f;
        float randTime;
        while (true)
        {
            randTime = Random.Range(minDelay, maxDelay);
            while (timer < randTime)
            {
                yield return null;
                if(!pause)
                timer += Time.deltaTime;
            }
            if (magCount <= 0)
            {
                yield return StartCoroutine(Reload()); //Run the reload and then resume
            }
            Instantiate(shotPattern, gunPoint.transform.position, gunPoint.transform.rotation, null);
            anim.SetTrigger("Fire");
            magCount--;
            timer = 0f;
        }
    }

    IEnumerator Reload()
    {
        anim.SetTrigger("Reload");
        float timer = 0f;
        while(timer < reloadTime)
        {
            magCount = magSize;
            yield return null;
        }
    }



    public void BeGone()
    {
        StopAllCoroutines();
    }
}
