using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRoom3D : MonoBehaviour
{
    [SerializeField] bool useCustomLens = false;
    public float customLens = 20f;


    //The location and size of the camera boundaries //deserialized because not needed to be filled in
    private float xMin;
    private float yMin;
    private float xMax;
    private float yMax;
    [Tooltip("How tall is the room the base is in (0 for top down angle)")]
    [SerializeField] private float roomHeight;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private GameObject player;
    [SerializeField] internal GameObject tracker;
    [SerializeField] private bool update = false;
    [SerializeField] private GameObject projectionRect;
    private CinemachineFramingTransposer transposer;
    // Start is called before the first frame update

    private float xMag;
    private float yMag;
    private Vector3 xComp, yComp, oyComp;
    private Vector3 UR, UL, DL, DR;
    private Plane camPlane, camX, camY;


    void Start()
    {

        Vector3 camForward = Camera.main.transform.forward, camUp = Camera.main.transform.up; //camera basis Vectors
        //I need to construct the plane here
        projectionRect.transform.rotation = Quaternion.LookRotation(camForward, camUp);
        //projectionRect.transform.RotateAround(projectionRect.transform.position, projectionRect.transform.right, projectionAngle - projectionRect.transform.rotation.eulerAngles.x);

        player = PlayerBody.Instance().gameObject;
        cam = FindObjectOfType<CinemachineVirtualCamera>();

        //Get the scale right
            //Get the corners
        UR = transform.position + new Vector3(transform.localScale.x / 2, 0, transform.localScale.z / 2);
        DR = transform.position + new Vector3(transform.localScale.x / 2, 0, -transform.localScale.z / 2);
        UL = transform.position + new Vector3(-transform.localScale.x / 2, 0, transform.localScale.z / 2);
        DL = transform.position + new Vector3(-transform.localScale.x / 2, 0, -transform.localScale.z / 2);
        //Project corners to view plane
        Vector3 URP = projectionRect.transform.position + Vector3.ProjectOnPlane(UR, projectionRect.transform.forward);
        Vector3 DRP = projectionRect.transform.position + Vector3.ProjectOnPlane(DR, projectionRect.transform.forward);
        Vector3 ULP = projectionRect.transform.position + Vector3.ProjectOnPlane(UL, projectionRect.transform.forward);
        Vector3 DLP = projectionRect.transform.position + Vector3.ProjectOnPlane(DL, projectionRect.transform.forward);
        //Figure out which is "up"
        float ULU, URU, DRU, DLU;
        ULU = Vector3.Dot(ULP, projectionRect.transform.up);
        URU = Vector3.Dot(URP, projectionRect.transform.up);
        DLU = Vector3.Dot(DLP, projectionRect.transform.up);
        DRU = Vector3.Dot(DRP, projectionRect.transform.up);

        if(Mathf.Max(ULU, URU, DRU, DLU) == ULU)
        {
            UL += new Vector3(0, roomHeight, 0);
            projectionRect.transform.position = (UL + DR)/2; 
            camPlane.SetNormalAndPosition(projectionRect.transform.forward, projectionRect.transform.position);
            camX.SetNormalAndPosition(projectionRect.transform.up, projectionRect.transform.position);
            camY.SetNormalAndPosition(projectionRect.transform.right, projectionRect.transform.position);
            URP = camPlane.ClosestPointOnPlane(UR);
            DRP = camPlane.ClosestPointOnPlane(DR);
            ULP = camPlane.ClosestPointOnPlane(UL);
            DLP = camPlane.ClosestPointOnPlane(DL);
            projectionRect.transform.position = (ULP + DRP) / 2;
            yComp = camY.ClosestPointOnPlane(ULP);
            oyComp = camY.ClosestPointOnPlane(DRP);
            xComp = camX.ClosestPointOnPlane(URP);
        }
        else if(Mathf.Max(ULU, URU, DRU, DLU) == DLU)
        {
            DL += new Vector3(0, roomHeight, 0);
            projectionRect.transform.position = (UR + DL) / 2;
            camPlane.SetNormalAndPosition(projectionRect.transform.forward, projectionRect.transform.position);
            camX.SetNormalAndPosition(projectionRect.transform.up, projectionRect.transform.position);
            camY.SetNormalAndPosition(projectionRect.transform.right, projectionRect.transform.position);
            URP = camPlane.ClosestPointOnPlane(UR);
            DRP = camPlane.ClosestPointOnPlane(DR);
            ULP = camPlane.ClosestPointOnPlane(UL);
            DLP = camPlane.ClosestPointOnPlane(DL);
            projectionRect.transform.position = (URP + DLP) / 2;
            yComp = camY.ClosestPointOnPlane(URP);
            oyComp = camY.ClosestPointOnPlane(DLP);
            xComp = camX.ClosestPointOnPlane(ULP);
        }
        else if(Mathf.Max(ULU, URU, DRU, DLU) == URU)
        {

            UR += new Vector3(0, roomHeight, 0);
            projectionRect.transform.position = (UR + DL) / 2;
            camPlane.SetNormalAndPosition(projectionRect.transform.forward, projectionRect.transform.position);
            camX.SetNormalAndPosition(projectionRect.transform.up, projectionRect.transform.position);
            camY.SetNormalAndPosition(projectionRect.transform.right, projectionRect.transform.position);
            URP = camPlane.ClosestPointOnPlane(UR);
            DRP = camPlane.ClosestPointOnPlane(DR);
            ULP = camPlane.ClosestPointOnPlane(UL);
            DLP = camPlane.ClosestPointOnPlane(DL);
            projectionRect.transform.position = (URP + DLP) / 2;
            yComp = camY.ClosestPointOnPlane(URP);
            oyComp = camY.ClosestPointOnPlane(DLP);
            xComp = camX.ClosestPointOnPlane(ULP);
        }
        else
        {
            DR += new Vector3(0, roomHeight, 0);
            projectionRect.transform.position = (UL + DR) / 2;
            camPlane.SetNormalAndPosition(projectionRect.transform.forward, projectionRect.transform.position);
            camX.SetNormalAndPosition(projectionRect.transform.up, projectionRect.transform.position);
            camY.SetNormalAndPosition(projectionRect.transform.right, projectionRect.transform.position);
            URP = camPlane.ClosestPointOnPlane(UR);
            DRP = camPlane.ClosestPointOnPlane(DR);
            ULP = camPlane.ClosestPointOnPlane(UL);
            DLP = camPlane.ClosestPointOnPlane(DL);
            yComp = camY.ClosestPointOnPlane(ULP);
            oyComp = camY.ClosestPointOnPlane(DRP);
            xComp = camX.ClosestPointOnPlane(URP);
            projectionRect.transform.position = (ULP + DRP) / 2;
        }
        projectionRect.transform.localScale = new Vector3((projectionRect.transform.position - xComp).magnitude * 2, (projectionRect.transform.position - yComp).magnitude + (projectionRect.transform.position - oyComp).magnitude, 1);

        tracker.transform.localScale = Vector3.one;

        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        float lensSize = 0;
        if (Camera.main.orthographic)
        {
            lensSize = cam.m_Lens.OrthographicSize; //vertical camera size
        }
        else
        {
            float VFOV = Camera.main.fieldOfView; //Vertical field of view
            float dist = transposer.m_CameraDistance;
            //if VFOV is 60, and dist = 10
            //tan(30) = a/10
            //dist(tan(VFOV/2))
            lensSize = Mathf.Abs(dist * Mathf.Tan(VFOV / 2 * Mathf.Deg2Rad));
        }
        if (useCustomLens)
            lensSize = customLens;

        float aspectLPlusRatio = Camera.main.aspect;
        //xMin is left edge of box + half the x dimension of the camera (which is aspectRatio * the vertical size)
        //Center of camera should not move past the point where the left edge leaves the room, xMin is the leftmost the center of the camera can be
        xMin = 0 - projectionRect.transform.localScale.x / 2 + aspectLPlusRatio * lensSize;
        //yMin is the same but vertically
        yMin = 0 - projectionRect.transform.localScale.y / 2 + lensSize;
        //xMax is from the right side
        xMax = 0 + projectionRect.transform.localScale.x / 2 - aspectLPlusRatio * lensSize;
        //and so on
        yMax = 0 + projectionRect.transform.localScale.y / 2 - lensSize;

        if (yMax < yMin)
        {
            yMax = 0;
            yMin = 0;
        }
        if (xMax < xMin)
        {
            xMax = 0;
            xMin = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;
        if (update)
        {
            Start();
            update = false;
        }
        tracker.transform.position = player.transform.position;
        tracker.transform.rotation = projectionRect.transform.rotation;
        tracker.transform.position = camPlane.ClosestPointOnPlane(tracker.transform.position);


        yMag = camX.GetDistanceToPoint(tracker.transform.position);
        xMag = camY.GetDistanceToPoint(tracker.transform.position);

        /*
        tUP = projectionRect.transform.up;
        tRIGHT = projectionRect.transform.right;

        if(Vector3.Dot(tUP, yComp) < 0)
        {
            yMag = -yMag;
        }

        if(Vector3.Dot(tRIGHT, xComp) < 0)
        {
            xMag = -xMag;
        }
        */
        xMag = Constrain(xMag, xMin, xMax);
        yMag = Constrain(yMag, yMin, yMax);


        tracker.transform.position = projectionRect.transform.position + projectionRect.transform.right*xMag + projectionRect.transform.up*yMag;


    }

    private float Map(float a, float b, float c, float d, float e)
    {
        return (a * (c - b) / (e - d));
    }

    private float Constrain(float val, float min, float max)
    {
        //no need to else because return stops the function in it's tracks (and on a basic level, else adds extra jump instructions, so I'm saving on code size too!)
        if (val < min)
            return min;
        if (val > max)
            return max;
        return val;
    }

   

}
