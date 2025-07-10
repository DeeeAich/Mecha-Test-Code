using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorRoomMover : MonoBehaviour
{
    public GameObject movingRoom;
    public GameObject endRoom;
    public Transform stopPlatformPos;
    public GameObject transitionTile;
    public GameObject currentTileStartPoint;
    public List<GameObject> currentTiles;



    public Animator mainPlatform;
    public GameObject mainPlatformCogs;

    public bool isMoving;
    public bool objectiveCompleted;
    public bool reachedEnd;

    public float moveSpeed;
    public float cogSpeed;
    public float spawnInterval;
    public float spawnTimer;


    public bool testTrigger;

    private void Start()
    {
        endRoom.SetActive(false);
    }
    void FixedUpdate()
    {
        if (testTrigger)
        {
            StartMoving();
        }


        if (isMoving)
        {
            Moving();
        }
    }

    public void StartMoving()
    {
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        mainPlatform.SetBool("Close", true);
        yield return new WaitForSeconds(1);
        isMoving = true;
    }
    void Moving()
    {
        //moving platform
        movingRoom.transform.Translate(Vector3.down * Time.fixedDeltaTime * moveSpeed);

        //rotating cog
        mainPlatformCogs.transform.Rotate(mainPlatformCogs.transform.rotation.x - (Time.fixedDeltaTime * cogSpeed), 0, 0);

        //spawning tiles
        spawnTimer += Time.fixedDeltaTime;


        if (endRoom.activeInHierarchy)
        {
            if(stopPlatformPos.position.y <= 0)
            {
                StopMoving();
            }
        }
        else
        {
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0;
                if (objectiveCompleted)
                {
                    SpawnEndTile();
                }
                else
                {
                    SpawnTransitionTile();
                }
            }
        }
    }


    public void SpawnTransitionTile()
    {
        //spawn tile at start point
        GameObject newTile = null;
        newTile = Instantiate(transitionTile);
        newTile.transform.position = currentTileStartPoint.transform.position;
        newTile.transform.parent = movingRoom.transform;
        newTile.transform.localRotation = Quaternion.Euler(0, 90, 0);

        //set new start point
        currentTileStartPoint = newTile.transform.GetChild(0).gameObject;

        //add to list
        currentTiles.Add(newTile);

        //if list over 4, delete oldest
        if (currentTiles.Count > 3)
        {
            if (currentTiles[0] != null)
                Destroy(currentTiles[0].gameObject);
            currentTiles.RemoveAt(0);
        }
    }

    public void SpawnEndTile()
    {
        endRoom.SetActive(true);
        endRoom.transform.position = currentTileStartPoint.transform.position;
    }

    public void StopMoving()
    {
        isMoving = false;
        mainPlatform.SetBool("Close", false);
        reachedEnd = true;
    }
}
