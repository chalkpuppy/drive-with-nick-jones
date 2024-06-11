using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LetterCollector : MonoBehaviour
{
    public Color collectedColor = Color.green;
    private GameManager gameManager;
    private List<string> lettersToCollect;
    [SerializeField] private GameObject particleEffectPrefab;
    private PlayerController playerController;
    [SerializeField] private GameObject winPopup;

    // Reference to the parent object containing UI letters
    [SerializeField] private GameObject lettersParent;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            lettersToCollect = new List<string>(gameManager.lettersToCollect);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }

        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found!");
        }

        if (winPopup != null)
        {
            winPopup.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            string letterName = other.gameObject.name.ToLower();
            string letter = letterName[0].ToString();

            Debug.Log($"Collected letter: {letter}");
            Debug.Log($"Current letters to collect: {string.Join(", ", lettersToCollect)}");

            if (particleEffectPrefab != null)
            {
                Instantiate(particleEffectPrefab, other.transform.position, Quaternion.identity);
            }

            if (lettersToCollect.Contains(letter))
            {
                lettersToCollect.Remove(letter);

                UpdateUI(letter); // Update UI for collected letter

                Debug.Log($"Letter {letter} found and removed. Remaining letters: {string.Join(", ", lettersToCollect)}");

                if (lettersToCollect.Count == 0)
                {
                    WinGame();
                }
            }
            else
            {
                Debug.Log($"Letter {letter} not found in the list. Losing a heart.");
                playerController.LoseHeart();
            }

            Destroy(other.gameObject);
        }
    }

    private void WinGame()
    {
        playerController.StopGame();

        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
    }

    private void UpdateUI(string letter)
    {
        // Convert the collected letter to uppercase
        string upperCaseLetter = letter.ToUpper();

        // Find the UI letter corresponding to the collected letter
        Transform uiLetter = lettersParent.transform.Find(upperCaseLetter);

        if (uiLetter != null)
        {
            // Check if the letter has already been collected
            if (!uiLetter.gameObject.name.EndsWith("Collected"))
            {
                // Change the name of the UI letter to indicate it's collected
                uiLetter.gameObject.name += "Collected";

                // Change the color of the text letter to green
                TextMeshProUGUI letterText = uiLetter.GetComponent<TextMeshProUGUI>();
                if (letterText != null)
                {
                    letterText.color = collectedColor;
                }
            }
        }
        else
        {
            Debug.LogWarning($"UI Letter for '{upperCaseLetter}' not found.");
        }
    }
}
