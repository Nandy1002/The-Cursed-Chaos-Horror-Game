using UnityEngine;

public class JuiceCanItem : PickUpItem 
{
    private GameObject openCan;
    private GameObject closedCan;
    private void Awake(){
        canMultiPick = false;
        timesOfUse = 5;
        closedCan = transform.GetChild(0).gameObject;
        openCan = transform.GetChild(1).gameObject;
        openCan.SetActive(false);
        closedCan.SetActive(true);
    }
    //try avoid using start here
    public override void OnUse(){
        if(playerAlreadyHasItem && timesOfUse > 0){
            Debug.Log("Using Juice Can");
            timesOfUse--;
            closedCan.SetActive(false);
            openCan.SetActive(true);
        }
    }
    public override void OnDrop()
    {
        if(playerAlreadyHasItem){
            Debug.Log("Dropping Juice Can");
            pickUpItemParent.RemovePickUpItem();
            playerAlreadyHasItem = false;
        }
    }
}
