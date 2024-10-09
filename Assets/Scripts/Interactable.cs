using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //[SerializeField] private GameObject interactInstructionsParent;
    [SerializeField] protected TextMeshProUGUI interactInstructions;
    private void Start(){
        //interactInstructions = GameObject.FindWithTag("UIElement").GetComponent<TextMeshProUGUI>();
        interactInstructions.gameObject.SetActive(false);
    }
    protected void OnInteract(){
        //Debug.Log("Interacted with " + gameObject.name);
        //interactInstructions.gameObject.SetActive(false);
    }
    protected void CanInteractVisual(){
        interactInstructions.gameObject.SetActive(true);
    }
}
