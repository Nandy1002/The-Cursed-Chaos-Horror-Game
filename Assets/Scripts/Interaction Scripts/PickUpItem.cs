using StarterAssets;
using TMPro;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] protected PickUpItemSO itemSO;
    [SerializeField] private TextMeshProUGUI pickUpInstructions;
    [SerializeField] protected int timesOfUse;
    [SerializeField] protected bool playerAlreadyHasItem;
    [SerializeField] protected bool canMultiPick;
    protected IPickUpItemParent pickUpItemParent;
    private void Start(){
        pickUpInstructions.gameObject.SetActive(false);
        playerAlreadyHasItem = false;
        //find the ui element in the hierarchy
        Transform canvas = GameObject.Find("Canvas").transform;
        if(canvas == null){
            Debug.LogError("Canvas not found");
        }else{
            pickUpInstructions = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    public void OnPickUpByPlayer(IPickUpItemParent parent,PickUpItem pickUpItem){
        pickUpItemParent?.RemovePickUpItem();
        pickUpItemParent = parent;
        if(parent.HasPickUpItem()){
            Debug.LogError("PickUpItemParent already has a PickUpItem");
        }
        pickUpItemParent.SetPickUpItem(pickUpItem);
        
        transform.position = parent.GetPickUpItemFollowTransform().position;
        transform.localPosition = Vector3.zero;
        playerAlreadyHasItem = true;
    }
    public IPickUpItemParent GetPickUpItemParent(){
        return pickUpItemParent;
    }
    // public static PickUpItem SpwanPickUpItem(PickUpItemSO itemSO, IPickUpItemParent parent){
    //     GameObject pickUpItemObject = Instantiate(itemSO.itemPrefab);
    //     Transform pickUpItemTransform = pickUpItemObject.transform;
    //     PickUpItem pickUpItem = pickUpItemTransform.GetComponent<PickUpItem>();
    //     pickUpItem.OnPickUpByPlayer(parent);
    //     return pickUpItem;
    // }
    protected virtual void AddToInventory(){

    }
    public virtual void OnUse(){
        if(playerAlreadyHasItem && timesOfUse > 0){
                Debug.Log("Using Item");
                timesOfUse--;
        }
    }
    public virtual void OnDrop(){
        if(playerAlreadyHasItem){
            Debug.Log("Dropping Item");
            pickUpItemParent.RemovePickUpItem();
            playerAlreadyHasItem = false;
        }
    }
    public void CanPickUpVisual(){
        if(!playerAlreadyHasItem){
            pickUpInstructions.text = "Press [E] to pick up";
            pickUpInstructions.gameObject.SetActive(true);
        }else{
            pickUpInstructions.gameObject.SetActive(false);
        }
    }
    public bool GetPlayerAlreadyHasItem(){
        return playerAlreadyHasItem;
    }
}
