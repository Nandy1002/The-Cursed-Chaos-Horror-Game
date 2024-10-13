using System.Collections;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected bool afterInteract=false;
    //[SerializeField] private GameObject interactInstructionsParent;
    [SerializeField] protected TextMeshProUGUI interactInstructions;
    private void Start(){
        //interactInstructions = GameObject.FindWithTag("UIElement").GetComponent<TextMeshProUGUI>();
        interactInstructions.gameObject.SetActive(false);
    }
    protected void OnInteract(){
        //Debug.Log("Interacted with " + gameObject.name);
        //interactInstructions.gameObject.SetActive(false);
        afterInteract = true;
        //wait for 5 seconds before the next interaction
        StartCoroutine(WaitForNextIneraction(5f));

    }
    protected void CanInteractVisual(){
        if(!afterInteract){
            interactInstructions.gameObject.SetActive(true);
        }else{
            interactInstructions.gameObject.SetActive(false);
        }
        
    }
     private IEnumerator WaitForNextIneraction(float time){
        yield return new WaitForSeconds(time);
        afterInteract = false;
    }
    public bool GetAfterInteract(){
        return afterInteract;
    }
}
