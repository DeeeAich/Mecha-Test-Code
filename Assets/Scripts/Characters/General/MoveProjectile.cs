using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectile : MonoBehaviour
{
    public Vector3 localVelocity;
    [SerializeField] private Vector3 globalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        globalVelocity = transform.rotation * localVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += globalVelocity * Time.fixedDeltaTime;
    }
}
