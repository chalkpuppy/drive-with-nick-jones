using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] jungleObstacles;
    public GameObject[] desertObstacles;

    public float[] laneXPositions = { -2.6f, 0f, 2.6f }; // X positions of the lanes

    public int obstaclesAhead = 4; // Number of obstacles to spawn ahead of the player
    public float obstacleSpawnInterval = 10f; // Interval between each obstacle spawn
    public float obstacleSpawnAheadDistance = 50f; // Distance ahead of the player to spawn obstacles (increased for further distance)
    public float switchZoneTime = 30f; // Time to switch between zones

    private bool isJungle = true; // Initially, the game starts in the jungle
    private Transform playerTransform;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextObstacleSpawnZ;
    private int lastLaneIndex = -1; // Index of the last used lane
    private float switchZoneTimer = 0f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nextObstacleSpawnZ = playerTransform.position.z + obstacleSpawnAheadDistance;
    }

    void Update()
    {
        if (playerTransform.position.z + obstacleSpawnAheadDistance >= nextObstacleSpawnZ)
        {
            for (int i = 0; i < obstaclesAhead; i++)
            {
                SpawnObstacle();
                nextObstacleSpawnZ += obstacleSpawnInterval;
            }
        }

        switchZoneTimer += Time.deltaTime;
        if (switchZoneTimer >= switchZoneTime)
        {
            SwitchZone();
            switchZoneTimer = 0f;
        }
    }

    void SpawnObstacle()
    {
        GameObject obstacleToSpawn;
        float spawnX;
        int laneIndex;

        // Choose a lane different from the last used lane
        do
        {
            laneIndex = Random.Range(0, laneXPositions.Length);
        } while (laneIndex == lastLaneIndex);

        lastLaneIndex = laneIndex; // Update the last used lane

        spawnX = laneXPositions[laneIndex];

        if (isJungle)
        {
            obstacleToSpawn = jungleObstacles[Random.Range(0, jungleObstacles.Length)];
        }
        else
        {
            obstacleToSpawn = desertObstacles[Random.Range(0, desertObstacles.Length)];
        }

        Vector3 spawnPosition = new Vector3(spawnX, 0f, nextObstacleSpawnZ);
        GameObject newObstacle = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
        activeObstacles.Add(newObstacle);
    }

    void SwitchZone()
    {
        isJungle = !isJungle;
        Debug.Log("Switched zone to " + (isJungle ? "Jungle" : "Desert"));
    }
}
