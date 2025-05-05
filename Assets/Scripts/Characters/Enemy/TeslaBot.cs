using AITree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBot : BehaviourTree
{
    public float calmVariance, calmStepDistance, approachDist;
    public Vector3 direction;
    public float speed;
    internal float speedBackup;
    internal float yPos;

    override internal void Awake()
    {
        base.Awake();
        direction = Random.onUnitSphere;
        direction.y = 0;
        direction.Normalize();

        yPos = transform.position.y;

        speedBackup = speed;
        //base.Awake();
        //AddOrOverwrite("player", player);
        //Vector3 rand = Random.onUnitSphere;
        //rand.y = 0;
        //rand.Normalize();
        //AddOrOverwrite("Velocity", rand);
        //root = new RootNode(this,
        //           new Sequence(
        //               new GetDVDBouncePoint("Velocity", "Destination"),
        //               new Approach("Destination", 0.1f, PositionStoreType.VECTOR3)
        //        )
        //    );
    }

    override internal void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    public override void Stop()
    {
        speed = 0f;
    }

    public override void Resume()
    {
        speed = speedBackup;
    }

    private void OnCollisionEnter(Collision collision)
    {

        Vector3 normal = collision.GetContact(0).normal.normalized; //normal.normalized;

        direction = direction - 2 * (Vector3.Dot(direction, normal)) * normal;
        direction.y = 0;
        direction.Normalize();
    }
}
