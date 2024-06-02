using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LetterCollector : MonoBehaviour
{
    // The string containing the word to be collected
    public string wordToCollect = "London";

    // List to keep track of collected letters
    private List<char> lettersToCollect;

    void Start()
    {
        // Split the wordToCollect string into individual letters
        lettersToCollect = new List<char>(wordToCollect.ToLower());
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Letter" tag
        if (other.CompareTag("Letter"))
        {
            // Check if the collided object's name is a letter to collect
            char collidedLetter = char.ToLower(other.gameObject.name[0]);
            if (lettersToCollect.Contains(collidedLetter))
            {
                // Mark the letter as collected
                lettersToCollect.Remove(collidedLetter);

                // Check if all letters are collected
                if (lettersToCollect.Count == 0)
                {
                    // Load the "AllCollected" scene
                    SceneManager.LoadScene("AllCollected");
                }
            }
            else
            {
                SceneManager.LoadScene("TryAgainScene");
            }
        }
        

    }
}
