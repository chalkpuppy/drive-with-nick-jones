using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Ground") || other.CompareTag("Letter"))
        {
            Destroy(other.gameObject);
        }
    }
}
