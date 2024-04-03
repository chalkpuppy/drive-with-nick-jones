using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Define a class to represent city data
    [System.Serializable]
    public class CityData
    {
        public string city;
        public string country;
        public bool is_collected;
        public string image_url;
    }

    // Define a class to represent continent data
    [System.Serializable]
    public class ContinentData
    {
        public string continent;
        public List<CityData> cities;
    }

    // Define a class to represent all continent data
    [System.Serializable]
    public class WorldData
    {
        public List<ContinentData> continents;
    }

    // JSON data structure
    private WorldData worldData;

    // List to store uncollected cities
    private List<CityData> uncollectedCities = new List<CityData>();

    void Start()
    {
        // Load the JSON file
        string jsonText = File.ReadAllText(Application.dataPath + "/JSON/cities.json");

        // Parse JSON data into the data structure
        worldData = JsonUtility.FromJson<WorldData>(jsonText);

        // Populate the list of uncollected cities
        foreach (ContinentData continent in worldData.continents)
        {
            foreach (CityData city in continent.cities)
            {
                if (!city.is_collected)
                {
                    uncollectedCities.Add(city);
                }
            }
        }

        // Select a random uncollected city and display its name
        CityData selectedCity = SelectRandomUncollectedCity();
        if (selectedCity != null)
        {
            Debug.Log("Selected city: " + selectedCity.city);
        }
    }

    // Function to select a random uncollected city
    public CityData SelectRandomUncollectedCity()
    {
        if (uncollectedCities.Count == 0)
        {
            Debug.LogError("No uncollected cities left!");
            return null;
        }

        // Select a random city from the list of uncollected cities
        int randomIndex = Random.Range(0, uncollectedCities.Count);
        CityData selectedCity = uncollectedCities[randomIndex];

        // Remove the selected city from the list to prevent selecting it again
        uncollectedCities.RemoveAt(randomIndex);

        return selectedCity;
    }
}
