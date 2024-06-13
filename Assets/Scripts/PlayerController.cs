using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject particles;
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    [SerializeField] private float minDistanceForSwipe = 20f;
    private float[] lanePositionsX = { -2.6f, 0f, 2.6f };

    private Animator animator;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;
    private int currentLaneIndex = 1;
    public GameObject[] hearts;
    private int heartsRemaining;
    [SerializeField] private GameObject gameOverPopup;
    private bool isGameOver = false;

    [SerializeField] private StoreCounter storeCounter; // Reference to StoreCounter

    // Cooldown variables
    [SerializeField] private float turnCooldownDuration = 0.5f;
    private float timeSinceLastTurn;

    // Audio variables
    public AudioSource audioSource;
    [SerializeField] private AudioClip turnSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        heartsRemaining = hearts.Length;


        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(false);
        }

        if (storeCounter == null)
        {
            storeCounter = FindObjectOfType<StoreCounter>(); // Find StoreCounter if not assigned
        }
    }

    private void Update()
    {
        if (isGameOver) return;

        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);
        DetectSwipe();
        DetectKeyboardInput();

        Vector3 targetPosition = new Vector3(lanePositionsX[currentLaneIndex], transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Update the time since last turn
        timeSinceLastTurn += Time.deltaTime;
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerDownPosition = Input.mousePosition;
            fingerUpPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            fingerDownPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerUpPosition = Input.mousePosition;
            ProcessSwipe();
        }
    }

    private void ProcessSwipe()
    {
        if (SwipeDistanceCheckMet() && IsHorizontalSwipe())
        {
            Vector3 swipeDirection = (fingerDownPosition.x - fingerUpPosition.x > 0) ? Vector3.left : Vector3.right;

            if (swipeDirection == Vector3.right)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
    }

    private bool IsHorizontalSwipe()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) > Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private bool SwipeDistanceCheckMet()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) > minDistanceForSwipe;
    }

    private void DetectKeyboardInput()
    {
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && timeSinceLastTurn >= turnCooldownDuration)
        {
            MoveRight();
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && timeSinceLastTurn >= turnCooldownDuration)
        {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        if (timeSinceLastTurn >= turnCooldownDuration && currentLaneIndex < lanePositionsX.Length - 1)
        {
            animator.SetTrigger("turnRight");
            currentLaneIndex++;
            timeSinceLastTurn = 0f; // Reset cooldown timer

            // Play turn sound
            PlayTurnSound();
        }
    }

    private void MoveLeft()
    {
        if (timeSinceLastTurn >= turnCooldownDuration && currentLaneIndex > 0)
        {
            animator.SetTrigger("turnLeft");
            currentLaneIndex--;
            timeSinceLastTurn = 0f; // Reset cooldown timer

            // Play turn sound
            PlayTurnSound();
        }
    }

    private void PlayTurnSound()
    {
        if (audioSource != null && turnSound != null)
        {
            audioSource.PlayOneShot(turnSound);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            animator.SetTrigger("break");
            LoseHeart();
        }
    }

    public void LoseHeart()
    {
        if (heartsRemaining > 0)
        {
            heartsRemaining--;
            Destroy(hearts[heartsRemaining]);
        }

        if (heartsRemaining == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        particles.SetActive(false);

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }

        forwardSpeed = 0;
        moveSpeed = 0;
        this.enabled = false;

        storeCounter.StopGame(); // Stop score counting
    }

    public void StopGame()
    {
        isGameOver = true;
        forwardSpeed = 0;
        moveSpeed = 0;
        this.enabled = false;
        storeCounter.StopGame(); // Stop score counting
    }
}
