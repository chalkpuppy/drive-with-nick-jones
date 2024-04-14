using UnityEngine;

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

    private void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Move forward
        transform.Translate(0, 0, forwardSpeed * Time.deltaTime);

        // Detect swipe gestures
        DetectSwipe();

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
                animator.SetTrigger("turnRight");
                currentLaneIndex = Mathf.Clamp(currentLaneIndex + 1, 0, lanePositionsX.Length - 1);
            }
            else
            {
                animator.SetTrigger("turnLeft");
                currentLaneIndex = Mathf.Clamp(currentLaneIndex - 1, 0, lanePositionsX.Length - 1);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Play the "Break" animation
            animator.SetTrigger("break");
        }
    }
}
