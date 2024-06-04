using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHP : MonoBehaviour {
    private bool canTakeDamage = true;
    [SerializeField] private float HP;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private PlayerMovement playerMovement; // Reference to the PlayerMovement script
    [SerializeField] private Slider healthSlider; // Reference to the health slider
    [SerializeField] private TextMeshProUGUI healthText; // Reference to the TMP text object

    public float GetHP {get {return HP;}}
    public float GetMaxHP {get {return maxHP;}}
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>(); // Get the PlayerMovement component from the same GameObject
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the slider value
        healthSlider.maxValue = maxHP;
        healthSlider.value = HP;

        // Update health text initially
        UpdateHealthText();
    }

    public void RestoreHP(float amout){
        HP += amout;
        if (HP > maxHP)
            HP = maxHP;
        UpdateHealthText();
    }

    public void ResetHP(){
        HP = maxHP;
        UpdateHealthText();
    }

    public void TakeDamage(float amount, Transform runnerTransform) {
        if(canTakeDamage) {
            HP -= amount;

            // Clamp HP to ensure it doesn't go below 0
            HP = Mathf.Max(HP, 0);

            // Update the health slider value
            

            // Update health text
            UpdateHealthText();

            if (HP <= 0)
                Die();
            else {
                StartCoroutine(DisableDamageForTwoSeconds());
                Vector2 backwardDirection = (transform.position - runnerTransform.position).normalized;

                Vector2 knockbackDirection = Vector2.up + backwardDirection;

                playerMovement.Knockback(knockbackDirection, 3f);
            }
        }

    }
    private IEnumerator DisableDamageForTwoSeconds() {
        canTakeDamage = false;
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();

        // Flicker effect loop
        for (int i = 0; i < 5; i++) {
            // Disable the player's sprite
            playerSpriteRenderer.enabled = false;
            
            // Wait for a short duration
            yield return new WaitForSeconds(0.1f);

            // Enable the player's sprite
            playerSpriteRenderer.enabled = true;

            // Wait for another short duration
            yield return new WaitForSeconds(0.1f);
        }

        // Enable damage after flickering effect
        canTakeDamage = true;

        // Wait for the remaining time (2 seconds - 5 * 0.1 seconds)
        yield return new WaitForSeconds(2f - 1f);
    }


    private void Die(){
        Destroy(gameObject);
    }


    private void UpdateHealthText() {
        healthSlider.value = HP;
        if (healthText != null) {
            float healthPercentage = HP / maxHP;
            healthText.text = $"{HP}/{maxHP}";
            
            // Set color based on health percentage
            // if (healthPercentage > 0.66f) {
                healthText.color = HexToColor("#137B15"); // Green when over 66%
            // } else if (healthPercentage > 0.33f) {
            //     healthText.color = Color.yellow; // Yellow between 33% and 66%
            // } else {
            //     healthText.color = Color.red; // Red below 33%
            // }
        }
    }

    private Color HexToColor(string hex) {
    Color color = Color.white;
    if (ColorUtility.TryParseHtmlString(hex, out color)) {
        return color;
    } else {
        Debug.LogError("Invalid hexadecimal color string: " + hex);
        return Color.white; // Default color
    }
}

}
