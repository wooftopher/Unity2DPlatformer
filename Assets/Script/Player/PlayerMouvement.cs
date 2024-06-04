using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] public GameObject fireballPrefab;
    [SerializeField] private GameObject PlayerRespawnPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;
    [SerializeField] private float attackCooldown = 0.1f;

    [Header("Jump System")]
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpTime;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] public DashController dashController;
    [SerializeField] public AttackController attackController;
    
    private bool isWalled = false;
    private bool canAttack = true;
    private bool isGrounded;
    private bool isJumping;
    private bool isAttacking = false;
    private bool isMovementEnabled = true;
    private float jumpCounter;


    private Vector2 vecGravity;
    private PlayerInput playerInput;
    private PlayerHP playerHP;

    public bool IsAttacking { get { return isAttacking; } }

    private void Awake() {
        playerHP = GetComponent<PlayerHP>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        vecGravity = new Vector2(0, -Physics.gravity.y);
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        dashController.Initialize(playerInput, anim, rb, transform, this);
        attackController.Initialize(playerInput, anim, transform, this);
    }
    public void SetMovementEnabled(bool enabled) {
        isMovementEnabled = enabled;
    }

    public void SetIsAttacking(bool enable){
        isAttacking = enable;
    }


    private void Update() {
        // Debug.Log("moveinput : " + playerInput.Player.Move.ReadValue<Vector2>());

        if(isMovementEnabled) {
            if (!dashController.IsDashing){
                HandleXMouvement();
                HandleJump();
                attackController.HandleAttack();
                // HandleAttack();

            }
            dashController.HandleDash(isAttacking);
        }
        PlayerRespawn();
    }

    private void PlayerRespawn(){
        if (playerInput.Player.Respawn.triggered) {
            transform.position = PlayerRespawnPoint.transform.position;
            playerHP.ResetHP();
            dashController.ResetDP();
            attackController.ResetAP();
        }
    }

    private void HandleXMouvement() {
        Vector2 horizontalInput = playerInput.Player.Move.ReadValue<Vector2>();

        // x movement
        rb.velocity = new Vector2(horizontalInput.x * speed, rb.velocity.y);
        anim.SetBool("run", Mathf.Abs(horizontalInput.x) > 0 && !isWalled);
            
        // flip player
        if (horizontalInput.x > 0.1f){
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput.x < -0.1f){
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    private void HandleJump(){
        float jumpInput = playerInput.Player.Jump.ReadValue<float>();
        //jump
        if (jumpInput > 0 && isGrounded){
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            anim.SetBool("isGrounded", false);
            isGrounded = false;
            isJumping = true;
            jumpCounter = 0;

        }
        //jumpMultiplier timer
        if (rb.velocity.y > 0 && isJumping){
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime)
                isJumping = false;

            rb.velocity += jumpMultiplier * Time.deltaTime * vecGravity;
        }
        if(jumpInput == 0){
            isJumping = false;
        }

        //gravity mutiplier
        if (rb.velocity.y < 0)
            rb.velocity -= fallMultiplier * Time.deltaTime * vecGravity;
    }

    //wall jump
    private void OnCollisionEnter2D(Collision2D collision) {
        // if (collision.gameObject.CompareTag("Ground")) {
        //     anim.SetBool("isGrounded", true);
        //     isGrounded = true;
        // }
        // else if (collision.gameObject.CompareTag("Fireball")) {
        //     // Do nothing if collided with fireball
        // }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        ICollectable collectable = other.GetComponent<ICollectable>();
        if (collectable != null) {
            // Debug.Log(collectable);
            collectable.CollectEffect(gameObject);
        }
    }

    public void Knockback(Vector2 direction, float force) {
        // Apply knockback force to the player
        isGrounded = false;
        StartCoroutine(DisableMovementForOneSecond());
        rb.velocity = Vector2.zero; // Reset player's current velocity
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
    
    private IEnumerator DisableMovementForOneSecond() {
        isMovementEnabled = false;
        yield return new WaitForSeconds(0.5f); // Disable movement for 1 second
        isMovementEnabled = true;
    }
    
    private void FixedUpdate() {
        // Perform the downward raycast
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, groundLayer);

        // Calculate the size of the box collider
        float boxColliderWidth = 0.6f; // Half of the desired width
        float boxColliderHeight = 0.7f; // Half of the desired height

        // Calculate the center position of the box collider based on the direction the player is facing
        float directionMultiplier = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 boxColliderCenter = (Vector2)transform.position + new Vector2(0.5f * directionMultiplier, -0.5f);

        // Perform the box cast
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxColliderCenter, new Vector2(boxColliderWidth, boxColliderHeight), 0f, groundLayer);

        // Update isGrounded based on the downward raycast
        if (hitDown.collider != null) {
            anim.SetBool("isGrounded", true);
            isGrounded = true;
        }

        // Update isWalled based on the box cast
        isWalled = hits.Length > 0;

        // Draw the raycasts in the scene view for visualization
        Debug.DrawRay(transform.position, Vector2.down * 1.3f, Color.green); // Downward raycast
        Debug.DrawRay(transform.position + new Vector3(0f, -0.5f), Vector2.right * directionMultiplier * boxColliderWidth, Color.blue); // Forward raycast
    }


}
