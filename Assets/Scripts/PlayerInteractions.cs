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
        Debug.DrawRay(transform.position, Vector3.forward * 2f, Color.red);
    }
    private void CanInteract(){
        RaycastHit hit;
        Vector3 direction = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;
        bool hitSomething = Physics.Raycast(transform.position, direction, out hit, 2f);
        if(hitSomething){
            if(hit.transform.TryGetComponent(out Interactable interactable)){
                Debug.Log("Can be Interacted with");
                Door door = interactable as Door;
                if(door != null){
                    OnInteracting(door);
                    door.CanInteractVisual();
                }
                VendingMachine vendingMachine = interactable as VendingMachine;
                if(vendingMachine != null){
                    OnInteracting(vendingMachine);
                    vendingMachine.CanInteractVisual();
                }
                
            }
        }
        //view the raycast
        //Debug.DrawRay(transform.position, direction * 1f, Color.red);
    }
    private void OnInteracting(Interactable interactable){
        if(inputs.interact && !interactable.GetAfterInteract()){
            Debug.Log("Interacted with " + interactable.gameObject.name);
            Door door = interactable as Door;
            if(door != null){ 
                door.OnInteract();
            }
            VendingMachine vendingMachine = interactable as VendingMachine;
            if(vendingMachine != null){
                vendingMachine.OnInteract();
            }
            inputs.interact = false;
        }
    }

    
}
