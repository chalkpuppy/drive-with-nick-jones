using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] jungleObstacles;
    public GameObject[] desertObstacles;

    public GameObject jungleParticleObject;
    public GameObject desertParticleObject;

    public Color jungleColor = Color.green;
    public Color desertColor = Color.yellow;

    public float[] laneXPositions = { -2.6f, 0f, 2.6f }; // X positions of the lanes

    public int obstaclesAhead = 4; // Number of obstacles to spawn ahead of the player
    public float spawnInterval = 10f; // Interval between each spawn
    public float spawnAheadDistance = 50f; // Distance ahead of the player to spawn
    public float switchZoneTime = 30f; // Time to switch between zones
    public float fogTransitionTime = 5f; // Time taken for fog color transition

    private bool isJungle = true; // Initially, the game starts in the jungle
    private Transform playerTransform;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float nextSpawnZ;
    private int lastLaneIndex = -1; // Index of the last used lane
    private float switchZoneTimer = 0f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nextSpawnZ = playerTransform.position.z + spawnAheadDistance;

        // Set initial particle colors
        SetParticleColors();

        // Set initial fog color
        RenderSettings.fogColor = isJungle ? jungleColor : desertColor;
    }

    void Update()
    {
        // Spawn obstacles
        if (playerTransform.position.z + spawnAheadDistance >= nextSpawnZ)
        {
            for (int i = 0; i < obstaclesAhead; i++)
            {
                SpawnObstacle();
                nextSpawnZ += spawnInterval;
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

        Vector3 spawnPosition = new Vector3(spawnX, 0f, nextSpawnZ);
        GameObject newObstacle = Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
        activeObstacles.Add(newObstacle);
    }

    void SwitchZone()
    {
        isJungle = !isJungle;
        Debug.Log("Switched zone to " + (isJungle ? "Jungle" : "Desert"));

        // Start the fog color transition coroutine
        StartCoroutine(ChangeFogColor(RenderSettings.fogColor, isJungle ? jungleColor : desertColor, fogTransitionTime));

        // Change particle colors after a delay
        Invoke("SetParticleColors", fogTransitionTime);
    }

    void SetParticleColors()
    {
        var jungleParticleMain = jungleParticleObject.GetComponent<ParticleSystem>().main;
        var desertParticleMain = desertParticleObject.GetComponent<ParticleSystem>().main;

        if (isJungle)
        {
            jungleParticleMain.startColor = jungleColor;
            desertParticleMain.startColor = jungleColor; // Change both particles to jungle color
        }
        else
        {
            jungleParticleMain.startColor = desertColor; // Change both particles to desert color
            desertParticleMain.startColor = desertColor;
        }
    }

    IEnumerator ChangeFogColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            RenderSettings.fogColor = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogColor = endColor; // Ensure the final color is set correctly
    }
}
