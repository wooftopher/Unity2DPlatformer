using UnityEngine;

public class Pineapple : MonoBehaviour, ICollectable {
    [SerializeField] private float restoreAmount = 10;
    private Animator animator;
    private void Awake(){
        animator = GetComponent<Animator>();
    }
    public void CollectEffect(GameObject player){
        Debug.Log("pick pineapple");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement.dashController.GetDP != playerMovement.dashController.GetMaxDP){
            animator.SetTrigger("Pickup");
            playerMovement.dashController.RestoreDP(restoreAmount);
            Destroy(gameObject, 0.75f);        
        }
    }
}
