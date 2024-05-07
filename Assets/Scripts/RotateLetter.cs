using UnityEngine;

public class RotateLetter : MonoBehaviour
{
    private float rotationSpeed = 150f; // Speed of rotation in degrees per second

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around the y-axis continuously
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
