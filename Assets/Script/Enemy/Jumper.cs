using UnityEngine;
using System.Collections;

public class Jumper : MonoBehaviour, IDamageable {
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player; // Reference to the player's transform
    [SerializeField] private float speed;
    [SerializeField] LayerMask groundlayer;
    [SerializeField] private float damage;
    [SerializeField] private float chaseDistance; // Distance at which the Runner starts chasing the player
    [SerializeField] private float chaseCooldown = 1f; // Cooldown period after being knocked back
    [SerializeField] private float jumpForce = 5f; // Force applied to jump
    private bool canChase = true; // Flag to control whether the runner can chase the player

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Assuming the Animator component is attached to the same GameObject
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update(){
        // Cast a box raycast downwards to check for ground
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position , Vector2.down, 0.5f, groundlayer);
        Debug.DrawRay(transform.position, Vector2.down * 1f, Color.green);
        
        // Check if the enemy is grounded and perform jump
        if (groundHit.collider != null) {
            Jump();
        }

        if (player == null || !canChase)
            return;

        // Calculate the distance between the Runner and the player
        float distanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);
        
        // Check if the player is within the chase distance
        if (distanceToPlayer < chaseDistance){
            // Calculate direction to move towards the player
            Vector2 direction = (player.position - transform.position).normalized;

            // Move the Runner towards the player
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

            // Optionally, flip the Runner's sprite based on movement direction
            if (direction.x < 0)
            {
                // If moving left, flip the sprite
                transform.localScale = new Vector2(-1f, 1f);
            }
            else if (direction.x > 0)
            {
                // If moving right, flip the sprite back to original orientation
                transform.localScale = Vector2.one;
            }

            // Set the "isRunning" parameter in the Animator to true
        }
        else {
            // Player is outside the chase distance, stop horizontal movement
            rb.velocity = new Vector2(0f, rb.velocity.y);

            // Set the "isRunning" parameter in the Animator to false
        }

        // Update animation parameters based on vertical velocity
        if (rb.velocity.y > 0)
        {
            // Going up
            anim.SetBool("isUp", true);
            anim.SetBool("isDown", false);
        }
        else if (rb.velocity.y < 0)
        {
            // Going down
            anim.SetBool("isUp", false);
            anim.SetBool("isDown", true);
        }
        else
        {
            // Neither going up nor down (idle)
            anim.SetBool("isUp", false);
            anim.SetBool("isDown", false);
        }

        // Set grounded animation parameter
        anim.SetBool("isGrounded", groundHit.collider != null);
        
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Debug.Log(distanceToPlayer);
        // Check if the distance between the Runner and the player is within the threshold
        if (distanceToPlayer < 1f) {
            // Trigger collision action
            HandlePlayerCollision();
        }
    }

    private void Jump() {
        // Apply jump force
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void HandlePlayerCollision() {
        // Runner KnockBack
        Vector2 knockbackDirection = (transform.position - player.position).normalized;
        Knockback(knockbackDirection);

        // Apply damage to the player
        PlayerHP playerHP = player.GetComponent<PlayerHP>();
        if (playerHP != null) {
            playerHP.TakeDamage(damage, gameObject.transform);
        }

        // Start cooldown for chasing
        StartCoroutine(ChaseCooldown());
    }

    private IEnumerator ChaseCooldown() {
        canChase = false; // Disable chasing
        yield return new WaitForSeconds(chaseCooldown);
        canChase = true; // Enable chasing after cooldown
    }

    public void Knockback(Vector2 direction) {
        // Apply knockback force to the Runner
        rb.velocity = Vector2.zero; // Reset velocity
        rb.AddForce(direction * 5f, ForceMode2D.Impulse);
    }

    // IDamageable interface method
    public void TakeDamage() {
        Destroy(gameObject);
    }
}
