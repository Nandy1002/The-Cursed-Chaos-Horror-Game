using System.Collections;
using UnityEngine;

public class SpwanFromVendMachine : MonoBehaviour
{
    [SerializeField] private float movementForce = 5f;
    private Rigidbody forcebody;
    private void Awake(){
        forcebody = GetComponent<Rigidbody>();
    }
    public void OnSpwanAction(){
        //StartCoroutine(SpwanTimeDelay(delay));
        forcebody.AddForce(movementForce * 10 * Time.deltaTime * Vector3.forward, ForceMode.Impulse);
    }
    
}
