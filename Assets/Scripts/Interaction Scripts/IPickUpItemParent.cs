using UnityEngine;

public interface IPickUpItemParent
{
    public Transform GetPickUpItemFollowTransform();
    public void SetPickUpItem(PickUpItem pickUpItem);
    public PickUpItem GetPickUpItem();
    public bool HasPickUpItem();
    public void RemovePickUpItem();
}
