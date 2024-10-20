using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] protected PickUpItemSO itemSO;
    private IPickUpItemParent pickUpItemParent;   
    protected void OnPickUpByPlayer(IPickUpItemParent pickUpItem){
        if(pickUpItemParent != null){
            pickUpItemParent.RemovePickUpItem();
        }
        pickUpItemParent = pickUpItem;
        if(pickUpItem.HasPickUpItem()){
            Debug.LogError("PickUpItemParent already has a PickUpItem");
        }
        pickUpItemParent.SetPickUpItem(this);
        transform.position = pickUpItem.GetPickUpItemFollowTransform().position;
        transform.localPosition = Vector3.zero;
    }
    protected void AddToInventory(){

    }
    protected void RemoveFromWorld(){

    }
    protected void OnUse(){

    }
    protected void OnDrop(){

    }
}
