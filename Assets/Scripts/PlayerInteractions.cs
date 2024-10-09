using StarterAssets;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private StarterAssetsInputs inputs;
    public PlayerInteractions instance;
    private CapsuleCollider interactionCollider; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance != null){
            Debug.LogError("There is more than one instance of Player in the scene");
        }
        instance = this;
        interactionCollider = GetComponent<CapsuleCollider>(); 
        inputs = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    private void Update()
    {
        CanInteract();
    }
    private void CanInteract(){
        RaycastHit hit;
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, 2f);
        if(hitSomething){
            if(hit.transform.TryGetComponent(out Interactable interactable)){
                Debug.Log("Can be Interacted with");
                OnInteracting(interactable);
            }
        }
        //view the raycast
        //Debug.DrawRay(transform.position, direction * 1f, Color.red);
    }
    private void OnInteracting(Interactable interactable){
        if(inputs.interact){
            Debug.Log("Interacted with " + interactable.gameObject.name);
            inputs.interact = false;
        }
    }

    
}
