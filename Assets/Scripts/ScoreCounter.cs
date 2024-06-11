using UnityEngine;
using TMPro;

public class StoreCounter : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI additionalScoreText;

    private int score = 0;
    private int additionalScore = 0;
    private float additionalScoreDuration = 2f;
    private float additionalScoreTimer = 0f;
    private bool isAdditionalScoreActive = false;

    void Start()
    {
        // Initialize UI text
        scoreText.text = score.ToString();
        additionalScoreText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Update score every second
        if (!isAdditionalScoreActive)
        {
            score += 1;
            scoreText.text = score.ToString();
        }

        // Check if additional score is active
        if (isAdditionalScoreActive)
        {
            additionalScoreTimer += Time.deltaTime;

            if (additionalScoreTimer >= additionalScoreDuration)
            {
                isAdditionalScoreActive = false;
                additionalScoreText.gameObject.SetActive(false);
                additionalScoreTimer = 0f;
            }
        }
    }

}
