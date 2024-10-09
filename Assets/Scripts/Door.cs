using UnityEngine;

public class Door : Interactable 
{
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    public new void OnInteract(){
        //change rotation to 90 degree over time
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime);
    }
}
