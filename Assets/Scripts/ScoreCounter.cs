using UnityEngine;
using TMPro;

public class StoreCounter : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score = 0;
    private int additionalScore = 0;
    private float additionalScoreDuration = 2f;
    private float additionalScoreTimer = 0f;
    private bool isAdditionalScoreActive = false;
    private float scoreUpdateInterval = 1f;
    private float scoreUpdateTimer = 0f;
    private bool isGameOver = false; // Add this flag

    void Start()
    {
        // Initialize UI text
        scoreText.text = score.ToString();
    }

    void Update()
    {
        if (isGameOver) return; // Stop updating if game is over

        // Update score every second
        scoreUpdateTimer += Time.deltaTime;
        if (scoreUpdateTimer >= scoreUpdateInterval)
        {
            score += 1;
            scoreText.text = score.ToString();
            scoreUpdateTimer = 0f;
        }

        // Check if additional score is active
        if (isAdditionalScoreActive)
        {
            additionalScoreTimer += Time.deltaTime;

            if (additionalScoreTimer >= additionalScoreDuration)
            {
                isAdditionalScoreActive = false;
                additionalScoreTimer = 0f;
            }
        }
    }

    public void StopGame()
    {
        isGameOver = true; // Set game over flag
    }
}
