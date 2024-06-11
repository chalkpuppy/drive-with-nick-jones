using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Swipe detection variables
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    [SerializeField] private float minDistanceForSwipe = 20f;

    // Lane related variables
    private float[] lanePositionsX = { -2.6f, 0f, 2.6f }; // Lane positions by x-coordinate

    // Animator reference
    private Animator animator;

    // Movement speed
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float moveSpeed = 5f;

    // Target lane index
    private int currentLaneIndex = 1; // Start in the middle lane

    // Hearts related variables
    public GameObject[] hearts;
    private int heartsRemaining;

    // Reference to the pop-up GameObject
    [SerializeField] private GameObject gameOverPopup;

    // Flag to check if the game is over
    private bool isGameOver = false;

    private void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Initialize hearts
        heartsRemaining = hearts.Length;

        // Ensure the game over pop-up is disabled at start
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameOver)
            return;

        // Move forward
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);

        // Detect swipe gestures
        DetectSwipe();

        // Detect keyboard input
        DetectKeyboardInput();

        // Move towards target lane position
        Vector3 targetPosition = new Vector3(lanePositionsX[currentLaneIndex], transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void DetectSwipe()
    {
        // Check for swipe input
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
            // Determine swipe direction
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
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        animator.SetTrigger("turnRight");
        currentLaneIndex = Mathf.Clamp(currentLaneIndex + 1, 0, lanePositionsX.Length - 1);
    }

    private void MoveLeft()
    {
        animator.SetTrigger("turnLeft");
        currentLaneIndex = Mathf.Clamp(currentLaneIndex - 1, 0, lanePositionsX.Length - 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Play the "Break" animation
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

        // Enable the game over pop-up
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }

        // Stop all movement
        forwardSpeed = 0;
        moveSpeed = 0;

        // Optionally, disable player input handling
        this.enabled = false;
    }

    public void StopGame()
    {
        isGameOver = true;

        // Stop all movement
        forwardSpeed = 0;
        moveSpeed = 0;

        // Optionally, disable player input handling
        this.enabled = false;
    }
}
