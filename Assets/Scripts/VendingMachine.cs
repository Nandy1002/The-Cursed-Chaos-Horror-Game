using System.Collections;
using UnityEngine;

public class VendingMachine : Interactable
{
    [SerializeField] private Animator vendFlap;
    [SerializeField] private PickUpItemSO spwanItemSO;
    [SerializeField] private Transform spawnPoint;
    private static readonly int isVending = Animator.StringToHash("IsVending");
    
    private void Awake(){
        vendFlap = vendFlap.GetComponent<Animator>();
    }

    public new void OnInteract()
    {
        base.OnInteract();
        vendFlap.SetTrigger(isVending);
        Debug.Log(vendFlap.name);
        if(afterInteract){
            //new WaitForSeconds(2f);
            StartCoroutine(ResetVendFlapTriggerAfterDelay(2f));
            Debug.Log("Vending");
            StartCoroutine(SpwanWithDelay(1f));
        }
        // StartCoroutine(WaitForStartVending(2f));
        // vendFlap.ResetTrigger(isVending);
    }
    public new void CanInteractVisual(){
        base.CanInteractVisual(); 
    }
    private IEnumerator ResetVendFlapTriggerAfterDelay(float delay){
    yield return new WaitForSeconds(delay);
    vendFlap.ResetTrigger(isVending);
    Debug.Log("Trigger reset after delay");
    }
    IEnumerator SpwanWithDelay(float delay){
        yield return new WaitForSeconds(delay);
        PickUpItem item = Instantiate(spwanItemSO.itemPrefab, spawnPoint.position, spawnPoint.rotation * Quaternion.Euler(0, 0, 90)).GetComponent<PickUpItem>();
        SpwanFromVendMachine spwanedItem = item.GetComponent<SpwanFromVendMachine>();
        spwanedItem.OnSpwanAction();
    }
}
