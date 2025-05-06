using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerNailGun : EnemyGun
{
    public bool pause = false;

    private GameObject player;

    private void Awake()
    {
        player = PlayerBody.Instance().gameObject;
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



    public override IEnumerator FireOnRepeat()
    {
        float timer = 0f;
        float randTime;
        while (true)
        {
            randTime = Random.Range(minDelay, maxDelay);
            while (timer < randTime)
            {
                yield return null;
                if (!pause)
                    timer += Time.deltaTime;
            }
            if (currentAmmo <= 0)
            {
                yield return StartCoroutine(Reload()); //Run the reload and then resume
            }
            if(Mathf.Acos(Vector3.Dot((player.transform.position - gunPoint.transform.position).normalized, gunPoint.transform.forward)) < Mathf.PI/6f)
            {
                Instantiate(shotPattern, gunPoint.transform.position, gunPoint.transform.rotation, null);
                anim.SetTrigger("Fire");
                currentAmmo--;
            }
            timer = 0f;
        }
    }

    IEnumerator Reload()
    {
        anim.SetTrigger("Reload");
        float timer = 0f;
        while (timer < reloadTime)
        {
            yield return null;
        }
        currentAmmo = ammoPerReload;
    }
}
