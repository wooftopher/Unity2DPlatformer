using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player's Transform
    public float smoothSpeed = 0.125f; // Speed at which the camera follows the player

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, transform.position.y, transform.position.z); // Keep the same Y and Z position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Smoothly move towards the desired position
            transform.position = smoothedPosition;
        }
    }
}
