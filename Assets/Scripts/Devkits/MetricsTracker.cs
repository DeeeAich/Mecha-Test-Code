using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct GameRuntimeMetrics
{
    public float playerRating;

    public float totalTimeTakenForRun;
    public float totalDamageTaken;
    public float totalDamagDealt;

    public List<LevelRuntimeMetrics> levelRuntimeMetricsList;
}

public struct LevelRuntimeMetrics
{
    public float timeTakenForLevel;

    public List<RoomRuntimeMetrics> roomRuntimeMetricsList;
}

public struct RoomRuntimeMetrics
{
    public string roomName;
    public string objectiveName;
    
    public float timeTaken;


    public List<TrackableEvent> events;
}
public struct TrackableEvent
{
    public string EventName;
    public float TimeStamp;

    public string Source, Target;
    public float Value;

    public AdditionalTrackableEventData[] additionalData;
}

public struct AdditionalTrackableEventData
{
    public string name;
    public string value;
}

public class MetricsTracker : MonoBehaviour
{
    private bool isTracking;
    public static MetricsTracker instance;

    public GameRuntimeMetrics currentGameRuntimeMetrics;
    public LevelRuntimeMetrics currentLevelRuntimeMetrics;
    public RoomRuntimeMetrics currentRoomRuntimeMetrics;

    private float roomTimer;
    private float levelTimer;
    private float gameTimer;

    public void RecordEvent(TrackableEvent trackableEvent) // this is for others to call to record damage data
    {
        if (currentGameRuntimeMetrics.levelRuntimeMetricsList[^1].roomRuntimeMetricsList[^1].timeTaken != 0)
        {
            currentGameRuntimeMetrics.levelRuntimeMetricsList[^1].roomRuntimeMetricsList[^1].events.Add(trackableEvent);
        }
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += HandleSceneChange();
        
        StartGameRuntimeTracking();
    }

    private void FixedUpdate()
    {
        if (isTracking)
        {
            gameTimer += Time.fixedDeltaTime;
            levelTimer += Time.fixedDeltaTime;
            roomTimer += Time.fixedDeltaTime;
        }
    }

    public void StartGameRuntimeTracking()
    {
        isTracking = true;

        currentGameRuntimeMetrics = new GameRuntimeMetrics();
        currentLevelRuntimeMetrics = new LevelRuntimeMetrics();
        currentRoomRuntimeMetrics = new RoomRuntimeMetrics();
    }

    private void StopGameRuntimeTracking()
    {
    }

    public void RoomStarted()
    {
        roomTimer = 0;
    }
    public void RoomCompleted(Room room)
    {
        currentRoomRuntimeMetrics.roomName = room.name;
        currentRoomRuntimeMetrics.objectiveName = room.primaryObjective.name;

        currentRoomRuntimeMetrics.timeTaken = roomTimer;
        roomTimer = 0;
        
        currentLevelRuntimeMetrics.roomRuntimeMetricsList.Add(currentRoomRuntimeMetrics);
        currentRoomRuntimeMetrics = new RoomRuntimeMetrics();
    }

    public void LevelCompleted()
    {
        currentLevelRuntimeMetrics.timeTakenForLevel = levelTimer;
        levelTimer = 0;
        
        currentGameRuntimeMetrics.levelRuntimeMetricsList.Add(currentLevelRuntimeMetrics);
        currentLevelRuntimeMetrics = new LevelRuntimeMetrics();
    }

    private UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode> HandleSceneChange()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                if(isTracking) StopGameRuntimeTracking();
                break;
            
            case 1:
                if (isTracking)
                {
                    StopGameRuntimeTracking();
                    StartGameRuntimeTracking();
                }
                break;
            
            default:
                
                if (isTracking)
                {
                    
                }

                break;
        }

        return null;
    }
}