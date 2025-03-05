using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObjectDelayed : MonoBehaviour
{
    GameObject target;
    public float delay = 1f;
    public float step = 0.01f;
    List<Vector3> positions;
    public bool usePlayer;
    public bool pauseAnimation;

    // Start is called before the first frame update
    void Start()
    {
        if(usePlayer)
        {
            target = PlayerBody.PlayBody().gameObject;
        }
        positions = new List<Vector3>();
        StartCoroutine(Follow());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Follow()
    {
        float timer = 0f;
        while (timer < delay)
        {
            float inTime = 0f;
            while(inTime < step)
            {
                inTime += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            positions.Add(target.transform.position);
        }
        while(true)
        {
            float inTime = 0f;
            while (inTime < step)
            {
                inTime += Time.deltaTime;
                yield return null;
            }
            if (!pauseAnimation)
            {
                transform.position = positions[0];
            }
            if (target == null)
                yield break;
            positions.RemoveAt(0);
            positions.Add(target.transform.position);
        }
    }
}
