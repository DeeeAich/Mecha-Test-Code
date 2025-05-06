using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverseerGunTilt : MonoBehaviour
{
    public GameObject turretFacing;
    GameObject player;
    Vector3 height;
    Vector3 distance;
    [SerializeField] Vector3 playerTargetingOffset;
    float distanceMag;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        height = Vector3.zero;
        height.y = transform.position.y - player.transform.position.y + playerTargetingOffset.y;
        distance = (player.transform.position + playerTargetingOffset) - transform.position;
        distance.y = 0;
        distanceMag = distance.magnitude;
        Vector3 myActualForeward = -turretFacing.transform.up;

        Vector3 constructedProjection = Vector3.zero - myActualForeward * distanceMag + height;

        transform.rotation = Quaternion.LookRotation(Vector3.Cross(constructedProjection, turretFacing.transform.right), constructedProjection);
    }
}
