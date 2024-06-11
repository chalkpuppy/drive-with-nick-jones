using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public string cityToCollect;
    public List<string> lettersToCollect;

    public GameObject letterPrefab; // Prefab for the letter UI element
    public TextMeshProUGUI aboveText; // Text element above the city name letters
    public GameObject lettersParent; // Parent GameObject to hold the letters
    public float letterSpacing = 2f; // Spacing between letters
    public float verticalOffset = 2f; // Vertical offset from the above text


    public GameObject planePrefab;
    public float planeSpawnInterval = 1f;
    public float planeSpawnTimer = 0f;
    public float planeSpawnZPosition = 0f;

    public GameObject[] lettersFolder;

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

    private void Awake()
    {
        InitializeLettersToCollect();
        GenerateCityName(cityToCollect);
    }

    void Start()
    {
        
        InvokeRepeating("SpawnPlane", 0f, planeSpawnInterval);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nextSpawnZ = playerTransform.position.z + spawnAheadDistance;

        // Set initial particle colors
        //SetParticleColors();

        // Set initial fog color
        //RenderSettings.fogColor = isJungle ? jungleColor : desertColor;
    }

    public void GenerateCityName(string cityName)
    {
        ClearCityName(); // Clear existing letters if any

        // Calculate starting position for the first letter
        float startX = -(cityName.Length - 1) * letterSpacing / 2;

        // Get the position to spawn the letters
        Vector3 spawnPosition = aboveText.rectTransform.position - new Vector3(0f, aboveText.rectTransform.sizeDelta.y / 2 + verticalOffset, 0f);

        // Position the letters horizontally centered with the specified spacing
        for (int i = 0; i < cityName.Length; i++)
        {
            char letter = cityName[i];
            Vector3 position = spawnPosition + new Vector3(startX + i * letterSpacing, 0f, 0f); // Calculate position based on startX and letterSpacing
            GameObject newLetter = Instantiate(letterPrefab, position, Quaternion.identity, lettersParent.transform); // Instantiate letter prefab as a child of the parent GameObject
            newLetter.name = letter.ToString().ToUpper(); // Name the GameObject with the letter name
            newLetter.GetComponent<TextMeshProUGUI>().text = letter.ToString().ToUpper(); // Set text of the UI element to the letter
        }
    }



    public void ClearCityName()
    {
        // Destroy all existing letters in the UI
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitializeLettersToCollect()
    {
        lettersToCollect = new List<string>();
        foreach (char letter in cityToCollect.ToLower())
        {
            lettersToCollect.Add(letter.ToString());
        }
    }

    private void FixedUpdate()
    {
        planeSpawnTimer += Time.deltaTime;
        if (planeSpawnTimer >= spawnInterval)
        {
            SpawnPlane();
            planeSpawnTimer = 0f;
        }
    }

    void Update()
    {


        if (playerTransform.position.z + spawnAheadDistance >= nextSpawnZ)
        {
            for (int i = 0; i < obstaclesAhead; i++)
            {
                SpawnObstacle();
                SpawnLetter(); // Call the method to spawn letters
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

    private void SpawnPlane()
    {
        GameObject newPlane = Instantiate(planePrefab, new Vector3(0f, 0f, planeSpawnZPosition), Quaternion.identity);


        planeSpawnZPosition += 10f; // Increment x position for next spawn
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

    void SpawnLetter()
    {
        // Choose a lane randomly
        int laneIndex = Random.Range(0, laneXPositions.Length);
        float spawnX = laneXPositions[laneIndex];

        // Choose a random letter prefab from the folder
        GameObject letterToSpawn = lettersFolder[Random.Range(0, lettersFolder.Length)];

        Vector3 spawnPosition = new Vector3(spawnX, 0f, nextSpawnZ);
        // Set the desired rotation
        Quaternion spawnRotation = Quaternion.Euler(90f, 180f, 0f);
        GameObject newLetter = Instantiate(letterToSpawn, spawnPosition, spawnRotation);
    }

    void SwitchZone()
    {
        isJungle = !isJungle;
        Debug.Log("Switched zone to " + (isJungle ? "Jungle" : "Desert"));

        // Start the fog color transition coroutine
        //StartCoroutine(ChangeFogColor(RenderSettings.fogColor, isJungle ? jungleColor : desertColor, fogTransitionTime));

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
