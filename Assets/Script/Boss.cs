using UnityEngine;

public class Boss : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2f; // Adjust this value to control the speed of the boss movement
    [SerializeField] private float targetHeight = 10f; // Adjust this value to set the desired height for the boss
    [SerializeField] private float idleRadius = 2f; // Adjust this value to set the radius of the circular movement
    [SerializeField] private float idleSpeed = 1f; // Adjust this value to set the speed of the circular movement

    private enum BossState {
        MovingUp,
        Idle
    }

    private BossState currentState = BossState.MovingUp;
    private Vector3 initialPosition;
    private float idleAngle = 0f;

    void Start() {
        // Set the initial position to the final height
        // initialPosition = transform.position + Vector3.up * targetHeight;
    }

    void Update() {
        switch (currentState) {
            case BossState.MovingUp:
                MoveUp();
                break;
            case BossState.Idle:
                PerformIdleMovement();
                break;
        }
    }

    void MoveUp() {
        // If the boss is still below the target height, move it upward
        if (transform.position.y < targetHeight) {
            // Calculate the new position
            Vector3 newPosition = transform.position + Vector3.up * moveSpeed * Time.deltaTime;

            // Clamp the position to ensure the boss doesn't overshoot the target height
            newPosition.y = Mathf.Min(newPosition.y, targetHeight);

            // Update the boss position
            transform.position = newPosition;
        } else {
            // Once the boss reaches the target height, switch to idle state
            initialPosition = transform.position;
            currentState = BossState.Idle;
        }
    }

    void PerformIdleMovement() {
        // Calculate the position for the circular movement relative to the final height
        float offsetX = Mathf.Cos(idleAngle) * idleRadius;
        float offsetY = Mathf.Sin(idleAngle) * idleRadius;
        Vector3 newPosition = initialPosition + new Vector3(offsetX, offsetY, 0f);

        // Update the boss position
        transform.position = newPosition;

        // Update the angle for the next frame
        idleAngle += idleSpeed * Time.deltaTime;

        // Ensure the angle stays within the range [0, 2*PI]
        if (idleAngle > Mathf.PI * 2) {
            idleAngle -= Mathf.PI * 2;
        }
    }
}
