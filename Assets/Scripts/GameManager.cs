using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    public Transform player; // Reference to the player object
    public GameObject[] jungleObstacles; // Array of jungle obstacles prefabs
    public GameObject[] desertObstacles; // Array of desert obstacles prefabs
    public float startDelay = 2f; // Delay before starting to spawn obstacles
    public float spawnInterval = 3f; // Initial interval between obstacle spawns
    public float minSpawnInterval = 1f; // Minimum interval between obstacle spawns
    public float intervalDecreaseRate = 0.1f; // Rate at which spawn interval decreases
    public float zoneSwitchInterval = 30f; // Interval to switch between zones (in seconds)
    public float zoneSwitchTimer = 0f; // Timer for zone switching

    private List<GameObject> activeObstacles = new List<GameObject>(); // List to keep track of active obstacles
    private Vector3[] spawnPoints = new Vector3[] { new Vector3(-2.6f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(2.6f, 0f, 0f) }; // Spawn points for obstacles
    private bool isJungleZone = true; // Flag to indicate if the jungle zone is active
    private float obstacleSpawnZ = 0f; // Z position to spawn the next obstacle

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(SpawnObstacles());
    }

    void Update()
    {
        // Update zone switching timer
        zoneSwitchTimer += Time.deltaTime;
        if (zoneSwitchTimer >= zoneSwitchInterval)
        {
            SwitchZone();
            zoneSwitchTimer = 0f;
        }

        // Decrease spawn interval over time
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - intervalDecreaseRate * Time.deltaTime);
    }

    IEnumerator SpawnObstacles()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // Choose a random spawn point
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Choose a random obstacle from the current zone
            GameObject obstaclePrefab = GetRandomObstacle();

            // Spawn the obstacle
            Vector3 spawnPosition = new Vector3(spawnPoint.x, spawnPoint.y, obstacleSpawnZ);
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            activeObstacles.Add(obstacle);

            // Update obstacle spawn position
            obstacleSpawnZ += spawnInterval;

            yield return null;
        }
    }

    GameObject GetRandomObstacle()
    {
        // Check which zone is active and return a random obstacle from that zone
        if (isJungleZone)
            return jungleObstacles[Random.Range(0, jungleObstacles.Length)];
        else
            return desertObstacles[Random.Range(0, desertObstacles.Length)];
    }

    void SwitchZone()
    {
        // Toggle between jungle and desert zones
        isJungleZone = !isJungleZone;
    }

    public void RemoveObstacle(GameObject obstacle)
    {
        // Remove obstacle from active obstacles list
        activeObstacles.Remove(obstacle);
        Destroy(obstacle);
    }
}
