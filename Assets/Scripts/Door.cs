using UnityEngine;

public class Door : Interactable 
{
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    public new void OnInteract()
    {
        Debug.Log("Door opened");
        base.OnInteract();
        //rotate the object for 90 degress over 1 second
        transform.Rotate(0, 90, 0, Space.Self);
    }
    public new void CanInteractVisual()
    {
        base.CanInteractVisual();
    } 
}
