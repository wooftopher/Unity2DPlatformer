
using UnityEngine;

public class Strawberry : MonoBehaviour, ICollectable {
    [SerializeField] private float restoreAmount = 10;
    private Animator animator;
    private void Awake(){
        animator = GetComponent<Animator>();
    }
    public void CollectEffect(GameObject player){
        Debug.Log("pick straw");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement.attackController.GetAP!= playerMovement.attackController.GetMaxAP){
            animator.SetTrigger("Pickup");
            playerMovement.attackController.RestoreAP(restoreAmount);
            Destroy(gameObject, 0.75f);        
        }
    }
}
