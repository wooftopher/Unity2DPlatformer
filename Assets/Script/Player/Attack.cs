using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;


public class AttackController : MonoBehaviour{

    [SerializeField] private float attackCooldown;
    [SerializeField] public GameObject fireballPrefab;
    [SerializeField] public float maxAttackPoint;
    [SerializeField] private float AttackCost;
    [SerializeField] private Slider attackPointSlider;
    [SerializeField] private TextMeshProUGUI attackPointsText;
    private float currentAttackPoint;
    private bool isAttackInputInitiated = false;
    private bool canAttack = true;
    private PlayerInput pi;
    private Transform tr;
    private Animator anim;
    private PlayerMovement playerMovement;
    private bool isAttackingCoroutineRunning = false;
    public float GetAP { get { return currentAttackPoint;}}
    public float GetMaxAP { get { return maxAttackPoint;}}

    public void Initialize(PlayerInput playerInput, Animator animator, Transform playerTransform, PlayerMovement movementScript) {
        pi = playerInput;
        anim = animator;
        // rb = rigidbody2D;
        tr = playerTransform;
        playerMovement = movementScript;
        currentAttackPoint = maxAttackPoint;
        UpdateAttackPointSlider();
    }
   
    public void ResetAP(){
        currentAttackPoint = maxAttackPoint;
        UpdateAttackPointSlider();
    }
    public void RestoreAP(float amout){
        currentAttackPoint += amout;
        if (currentAttackPoint > maxAttackPoint)
            currentAttackPoint = maxAttackPoint;
        UpdateAttackPointSlider();
    }

    public void HandleAttack(){
        float attackInput = pi.Player.Attack.ReadValue<float>();

        if (playerMovement.dashController.IsDashing)
            return;

        if (attackInput > 0 && !isAttackInputInitiated && canAttack && HasEnoughAP()){
            anim.SetBool("isAttacking", true);
            playerMovement.SetIsAttacking(true);
            isAttackInputInitiated = true;
            StartCoroutine(AttackCoroutine());
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }

        if (attackInput == 0){
            isAttackInputInitiated = false;
        }

        if (attackInput > 0 && isAttackInputInitiated && !canAttack){
            return;
        }

        if (attackInput == 0){
            playerMovement.SetIsAttacking(false);
            anim.SetBool("isAttacking", false);
        }
    }
    private bool HasEnoughAP() {
        return currentAttackPoint >= AttackCost;
    }

    private IEnumerator AttackCoroutine() {
        if (isAttackingCoroutineRunning)
        yield break;

        isAttackingCoroutineRunning = true;
        Transform fireballSpawnPoint = transform.Find("FireballSpawnPoint");

        while (playerMovement.IsAttacking && HasEnoughAP()) {
            currentAttackPoint -= AttackCost;
            UpdateAttackPointSlider();
            // Debug.Log("attackpont : " + currentAttackPoint);

            Vector2 moveInput = pi.Player.Move.ReadValue<Vector2>();
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            if (moveInput == Vector2.zero){
                fireball.GetComponent<Fireball>().SetDirection(tr.localScale.x > 0 ? Vector2.right : Vector2.left);
                if (tr.localScale.x < 0) {
                    SpriteRenderer fireballSpriteRenderer = fireball.GetComponent<SpriteRenderer>();
                    fireballSpriteRenderer.flipX = true;
                }
            }
            else
                fireball.GetComponent<Fireball>().SetDirection(moveInput);

            // Rotate the fireball sprite to match its direction
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            fireball.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            yield return new WaitForSeconds(attackCooldown);
        }
        isAttackingCoroutineRunning = false;


    }

    private IEnumerator AttackCooldown() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    private void UpdateAttackPointSlider() {
        // Update the UI slider value based on current dash points
        attackPointSlider.value = currentAttackPoint / maxAttackPoint;

        if (attackPointsText != null) {
            int roundedCurrentAttackPoints = Mathf.RoundToInt(currentAttackPoint);
            attackPointsText.text = $"{roundedCurrentAttackPoints}/{maxAttackPoint}";
        }
    }
}
