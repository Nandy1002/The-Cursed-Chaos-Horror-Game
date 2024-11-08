using System;
using StarterAssets;
using TMPro;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour,IPickUpItemParent
{
    // Private Variables
    private StarterAssetsInputs inputs;
    public PlayerInteractions instance;
    protected CapsuleCollider interactionCollider; 

    //  serialized variables
    [SerializeField] private Transform pickUpItemHoldPoint;
    [SerializeField] private PickUpItem pickUpItem;
    [SerializeField] private TextMeshProUGUI interactInstructions;
    
    // Events
    public event EventHandler OnPickUpItemGrabbed;

    // Methods
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
        CanPickUp();
        //Debug.DrawRay(transform.position, Vector3.forward * 2f, Color.red);
    }
    private void CanInteract(){
        Vector3 direction = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;
        bool hitSomething = Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f);
        if(hitSomething){
            if(hit.transform.TryGetComponent(out Interactable interactable)){
                Debug.Log("Can be Interacted with");
                Door door = interactable as Door;
                VendingMachine vendingMachine = interactable as VendingMachine;
                if(door != null){
                    OnInteracting(door);
                    door.CanInteractVisual();
                }
                else if(vendingMachine != null){
                    OnInteracting(vendingMachine);
                    vendingMachine.CanInteractVisual();
                }   
            }
        }else{
                interactInstructions.gameObject.SetActive(false);
        }
        //view the raycast
        //Debug.DrawRay(transform.position, direction * 1f, Color.red);
    }
    private void CanPickUp(){
        Vector3 direction = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;
        bool hitSomething = Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f);
        if(hitSomething){
            if(hit.transform.TryGetComponent(out PickUpItem pickUpItem)){
                Debug.Log("Can be Picked Up");
                OnPickingUp(pickUpItem);
                pickUpItem.CanPickUpVisual();
                
            }
        }
        //if player already have the object then still use pickup operation for other functions
        if(pickUpItem != null){
            OnPickingUp(pickUpItem);
        }
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
    private void OnPickingUp(PickUpItem pickUpItem){
        if(inputs.pickUp && !pickUpItem.GetPlayerAlreadyHasItem()){
            Debug.Log("Pickup Button Working");
            pickUpItem.OnPickUpByPlayer(this,pickUpItem);
            Debug.Log("Picked Up " + pickUpItem.name);
            inputs.pickUp = false;
        }
        if(inputs.pickUp && pickUpItem.GetPlayerAlreadyHasItem()){
            Debug.Log("Drop Button Working");
            pickUpItem.OnDrop();
            inputs.pickUp = false;
        }
        if(inputs.click && pickUpItem.GetPlayerAlreadyHasItem()){
            Debug.Log("Use Button Working");
            pickUpItem.OnUse();
            inputs.click = false;
        }
    }

    public Transform GetPickUpItemFollowTransform()
    {
        return pickUpItemHoldPoint;
    }

    public void SetPickUpItem(PickUpItem pickUpItem)
    {
        this.pickUpItem = pickUpItem;
        //freeze the total position of pickup item ridigbody constraints
        pickUpItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // pickup the prefab also with the help of pickupitem
        pickUpItem.transform.SetParent(pickUpItemHoldPoint);
        pickUpItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (pickUpItem != null){
            OnPickUpItemGrabbed?.Invoke(this, EventArgs.Empty);
        }
    }

    public PickUpItem GetPickUpItem()
    {
        return pickUpItem;
    }

    public bool HasPickUpItem()
    {
        return pickUpItem != null;
    }

    public void RemovePickUpItem()
    {
        if(pickUpItem == null){
            return;
        }else{
            pickUpItem.transform.SetParent(null);
            pickUpItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            pickUpItem.GetComponent<Rigidbody>().AddForce(transform.forward * 2f, ForceMode.Impulse);
            pickUpItem = null;
        }
    }
}
