using UnityEngine;

public class MoveAsParent : MonoBehaviour
{
    public Transform parentTransform;


    void Update()
    {
        // Move the object along the parent's x-axis but maintain its current position on other axes
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, parentTransform.position.z);
        transform.position = newPosition;
    }
}
