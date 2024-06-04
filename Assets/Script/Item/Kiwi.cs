using UnityEngine;

public class Kiwi : MonoBehaviour, ICollectable {
    [SerializeField] private float restoreAmount = 10;
    private Animator animator;
    private void Awake(){
        animator = GetComponent<Animator>();
    }
    
    public void CollectEffect(GameObject player){
        PlayerHP playerHP = player.GetComponent<PlayerHP>();
        if (playerHP.GetHP != playerHP.GetMaxHP){
            animator.SetTrigger("Pickup");
            playerHP.RestoreHP(restoreAmount);
            Destroy(gameObject, 0.75f);        
        }
    }
 
}
