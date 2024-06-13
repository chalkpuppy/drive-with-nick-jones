using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LetterCollector : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private AudioClip collectSound;

    public Color collectedColor = Color.green;
    private GameManager gameManager;
    private List<string> lettersToCollect;
    [SerializeField] private GameObject particleEffectPrefab;
    private PlayerController playerController;
    [SerializeField] private GameObject winPopup;

    [SerializeField] private GameObject lettersParent;
    [SerializeField] private StoreCounter storeCounter; // Reference to StoreCounter

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

        if (storeCounter == null)
        {
            storeCounter = FindObjectOfType<StoreCounter>(); // Find StoreCounter if not assigned
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Letter"))
        {
            audioSource.PlayOneShot(collectSound);
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
        else if (other.CompareTag("Obstacle"))
        {
            audioSource.PlayOneShot(crashSound);
            playerController.LoseHeart();
            playerController.LoseHeart();
            playerController.LoseHeart();
            Destroy(this);
        }

    }

    private void WinGame()
    {
        playerController.StopGame();
        storeCounter.StopGame(); // Stop score counting

        if (winPopup != null)
        {
            winPopup.SetActive(true);
        }
    }

    private void UpdateUI(string letter)
    {
        string upperCaseLetter = letter.ToUpper();
        Transform uiLetter = lettersParent.transform.Find(upperCaseLetter);

        if (uiLetter != null)
        {
            if (!uiLetter.gameObject.name.EndsWith("Collected"))
            {
                uiLetter.gameObject.name += "Collected";
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
