
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private TestSO testSO;

    private void Start(){
        Debug.Log(testSO.myString);
    }
}
