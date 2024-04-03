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
    [SerializeField] private float moveSpeed = 5f;

    // Target lane position
    private Vector3 targetLanePosition;

    private void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Detect swipe gestures
        DetectSwipe();

        // Move towards target lane position
        transform.position = Vector3.MoveTowards(transform.position, targetLanePosition, moveSpeed * Time.deltaTime);
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

            // Calculate target lane position
            int currentLaneIndex = GetCurrentLaneIndex();
            int nextLaneIndex = Mathf.Clamp(currentLaneIndex + (int)swipeDirection.x, 0, lanePositionsX.Length - 1);
            targetLanePosition = new Vector3(lanePositionsX[nextLaneIndex], transform.position.y, transform.position.z);

            // Trigger animation based on swipe direction
            if (swipeDirection == Vector3.right)
            {
                animator.SetTrigger("turnRight");
            }
            else
            {
                animator.SetTrigger("turnLeft");
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

    private int GetCurrentLaneIndex()
    {
        // Find closest lane index
        float currentX = transform.position.x;
        float minDistance = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < lanePositionsX.Length; i++)
        {
            float distance = Mathf.Abs(currentX - lanePositionsX[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
