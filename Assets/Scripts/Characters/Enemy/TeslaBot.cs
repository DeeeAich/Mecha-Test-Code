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
    internal Vector3 lastKnownLocation;
    internal List<Collider> colliders;

    override internal void Awake()
    {
        colliders = new List<Collider>();
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
        colliders.Add(collision.collider);
        direction = direction - 2 * (Vector3.Dot(direction, normal)) * normal;
        direction.y = 0;
        direction.Normalize();
        lastKnownLocation = transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        transform.position = lastKnownLocation; 
        
        Vector3 normal = collision.GetContact(0).normal.normalized; //normal.normalized;
        normal += Random.insideUnitSphere * 0.5f;
        direction = normal; //direction - 2 * (Vector3.Dot(direction, normal)) * normal;
        direction.y = 0;
        direction.Normalize();

        Debug.Log("did the thing");
    }

    private void OnCollisionExit(Collision collision)
    {
        colliders.Remove(collision.collider);
    }
}
