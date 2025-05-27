using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerWeaponsLook : MonoBehaviour
{
    public GameObject lookTarget;

    public float speed = 1f;
    [SerializeField] private bool _pause = false;
    internal Quaternion _lockedRotation;
    internal float _savedSpeed;

    public bool pause
    {
        get
        {
            return _pause;
        }
        set
        {
            _lockedRotation = transform.rotation;
            _pause = value;
            if(_pause)
            {
                _savedSpeed = speed;
                speed = 99f;
            }
            else
            {
                speed = _savedSpeed;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _savedSpeed = speed;
        if(lookTarget == null)
        lookTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(lookTarget == null)
        {
            enabled = false;
            return;
        }
        Vector3 lookDir = lookTarget.transform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, _pause ? _lockedRotation : Quaternion.LookRotation(Vector3.up, -lookDir), speed * Time.deltaTime);
    }

    internal void Pause(bool pause)
    {
        this.pause = pause;
    }

    public void SetRotationSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
