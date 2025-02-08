using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornLean : MonoBehaviour
{
    [SerializeField] internal float maxLean = 1f;
    [SerializeField] [Range(0f,1f)] internal float dampening = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.hideFlags = HideFlags.HideInHierarchy;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 forceAcc = Vector3.zero;
        forceAcc += EnvironmentalData.Instance.GetWind(transform.position);
        Vector3 forceAccZ = forceAcc;
        Vector3 forceAccX = forceAcc;
        forceAccZ.x = 0;
        forceAccX.z = 0;
        float xAngle = Vector3.Angle(Vector3.up, forceAccX);
        float zAngle = Vector3.Angle(Vector3.up, forceAccZ);

        xAngle = Mathf.Clamp(xAngle, 0, maxLean);
        zAngle = Mathf.Clamp(zAngle, 0, maxLean);

        Quaternion sway = Quaternion.identity;

        sway.eulerAngles = new Vector3(forceAccZ.z < 0 ? -zAngle : zAngle, 0, forceAccX.x > 0 ? -xAngle : xAngle);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, sway, dampening * Time.deltaTime);
    }
}
