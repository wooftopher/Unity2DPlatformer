using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;

public class DashController : MonoBehaviour {
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float maxDashPoints = 100f;
    [SerializeField] private float dashCost = 20f;
    [SerializeField] private float dashRegenerationRate = 10f;
    [SerializeField] private Slider dashPointSlider;
    [SerializeField] private TextMeshProUGUI dashPointsText;

    private bool isDashing = false;
    private Animator anim;
    private Rigidbody2D rb;
    private Transform tr;
    private PlayerInput pi;
    private PlayerMovement playerMovement;
    private float currentDashPoints;
    private float dashCooldownTimer;
    
    public float GetDP { get { return currentDashPoints;}}
    public float GetMaxDP { get { return maxDashPoints;}}
    public bool IsDashing { get { return isDashing; } }
    public bool CanDash { get { return currentDashPoints >= dashCost && !isDashing && dashCooldownTimer <= 0f; } }

    public void Initialize(PlayerInput playerInput, Animator animator, Rigidbody2D rigidbody2D,
                                    Transform playerTransform, PlayerMovement movementScript) {
        pi = playerInput;
        anim = animator;
        rb = rigidbody2D;
        tr = playerTransform;
        playerMovement = movementScript;
    }
    public void ResetDP(){
        currentDashPoints = maxDashPoints;
    }
    public void RestoreDP(float amout){
        currentDashPoints += amout;
        if (currentDashPoints > maxDashPoints)
            currentDashPoints = maxDashPoints;
        UpdateDashPointSlider();
    }


    private void Awake() {
        currentDashPoints = maxDashPoints;
    }

    private void Update() {
        // Removed HandleDash from Update since it's called by the input action now
    }

    public void HandleDash(bool isAttacking) {
        float DashInput = pi.Player.Dash.ReadValue<float>();

        if (DashInput > 0 && !isDashing && dashCooldownTimer <= 0f) {
            // Check if the player has enough dash points
            if (currentDashPoints >= dashCost) {
                if (isAttacking) {
                    // Stop the attack animation
                    playerMovement.SetIsAttacking(false);
                    anim.SetBool("isAttacking", false);
                }
                StartCoroutine(Dash());
                currentDashPoints -= dashCost;
            }
            else {
                Debug.Log("Not enough dash points!");
            }
        }

        // Update dash cooldown timer
        if (dashCooldownTimer > 0f) {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Regenerate dash points over time
        currentDashPoints = Mathf.Clamp(currentDashPoints + dashRegenerationRate * Time.deltaTime, 0f, maxDashPoints);
        UpdateDashPointSlider();
    }

    private IEnumerator Dash() {
        isDashing = true;
        anim.SetBool("isDashing", true);
        // playerMovement.SetMovementEnabled(false);
        
        // Disable gravity during the dash
        float originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // Preserve the current Y velocity
        float currentYVelocity = rb.velocity.y;

        // Calculate dash direction based on player's facing direction
        float dashDirection = tr.localScale.x > 0 ? 1f : -1f;
        Vector2 dashVector = new Vector2(dashDirection, 0).normalized * dashDistance;

        // Apply dash velocity
        rb.velocity = dashVector / dashDuration;

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        // Restore gravity
        rb.gravityScale = originalGravityScale;

        //     COOL FEATURE
        // Restore the player's Y velocity
        // rb.velocity = new Vector2(rb.velocity.x, currentYVelocity);

        isDashing = false;
        // playerMovement.SetMovementEnabled(true);

        // Set the dash animation state back to false
        anim.SetBool("isDashing", false);

        // Start dash cooldown
        dashCooldownTimer = dashCooldown;
    }
    private void UpdateDashPointSlider() {
        // Update the UI slider value based on current dash points
        dashPointSlider.value = currentDashPoints / maxDashPoints;

        if (dashPointsText != null) {
            int roundedCurrentDashPoints = Mathf.RoundToInt(currentDashPoints);
            dashPointsText.text = $"{roundedCurrentDashPoints}/{maxDashPoints}";
        }
    }
}
