using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneRotationClamp : MonoBehaviour
{
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    private float lockedXAngle;
    private float lockedYAngle;
    private float lockedZAngle;

    private void OnEnable()
    {
        lockedXAngle = transform.localEulerAngles.x;
        lockedYAngle = transform.localEulerAngles.y;
        lockedZAngle = transform.localEulerAngles.z;
    }

    void LateUpdate()
    {      
        if (lockX) { transform.localRotation = Quaternion.Euler(lockedXAngle, transform.localEulerAngles.y, transform.localEulerAngles.z); }
        if (lockY) { transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, lockedYAngle, transform.localEulerAngles.z); }
        if (lockZ) { transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, lockedZAngle); }
    }

}

public enum Axis
{
    x,y,z
}
