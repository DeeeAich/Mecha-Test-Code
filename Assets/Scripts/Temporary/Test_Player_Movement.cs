using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Player_Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 inputVector;

    private void Update()
    {
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        transform.Translate(transform.right * inputVector.x * speed);
        transform.Translate(transform.forward * inputVector.y * speed);

    }
}
