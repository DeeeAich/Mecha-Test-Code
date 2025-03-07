using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerWeaponsLook : MonoBehaviour
{
    public GameObject player;
    public float speed = 1f;
    internal bool pause = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || pause)
        {
            return;
        }
        Vector3 lookDir = player.transform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.up, -lookDir), speed * Time.deltaTime);
    }

    internal void Pause(bool pause)
    {
        this.pause = pause;
    }
}
