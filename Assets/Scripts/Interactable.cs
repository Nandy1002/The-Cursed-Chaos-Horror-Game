using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void OnInteract(){
        Debug.Log("Interacted with " + gameObject.name);
    }
}
