using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLook : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDir = transform.position - player.transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(Vector3.up, lookDir);
    }
}
