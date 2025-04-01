using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MissileDeployment : MonoBehaviour
{
    [SerializeField] float startTime = 1f/6f, interimTime = 1f/3f;
    [SerializeField] GameObject[] missiles;
    int fired = 0;
    float timer = 0f;
    float activeTimer;

    private void Start()
    {
        activeTimer = startTime;
    }

    private void FixedUpdate()
    {
        if(fired >= missiles.Length)
        {
            Destroy(gameObject);
        }
        else
        {
            timer += Time.fixedDeltaTime;
            if(timer > activeTimer)
            {
                missiles[fired].SetActive(true);
                missiles[fired].transform.parent = null;
                fired++;
                timer -= activeTimer;
                activeTimer = interimTime;
            }
        }
    }
}

