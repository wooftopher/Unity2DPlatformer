using UnityEngine;

public class Fireball : MonoBehaviour {
    private Rigidbody2D rb;
    [SerializeField] private float speed;

    private Vector2 direction;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate(){
        rb.velocity = direction * speed;
    }

    public void SetDirection(Vector2 newDirection) {
        direction = newDirection.normalized;
    }

    // Detect when the fireball becomes invisible to any camera
    private void OnBecameInvisible() {
        // Destroy the fireball when it's out of the camera's view
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider){
        // Debug.Log(collider.gameObject.name);
        IDamageable damageable = collider.GetComponent<IDamageable>();
        if (damageable != null){
            damageable.TakeDamage();
        }
            Destroy(gameObject);
    }
}
