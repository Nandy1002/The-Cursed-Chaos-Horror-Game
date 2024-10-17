using UnityEngine;

public class Door : Interactable 
{
    private Animator animator;
    private static readonly int isOpened = Animator.StringToHash("IsOpened");
    private static readonly int isClosed = Animator.StringToHash("IsClosed");
    [SerializeField] private bool isDoorOpen;
    private void Awake(){
        animator = GetComponent<Animator>();
        isDoorOpen = false;
    }
    public new void OnInteract()
    {
        base.OnInteract();
        //rotate the object for 90 degress over 1 second
        //transform.Rotate(0, 90, 0, Space.Self);
        if(isDoorOpen){
                animator.SetBool(isOpened,false);
                animator.SetTrigger(isClosed);
                isDoorOpen = false;
                //Debug.Log("Door opened");        
        }else{
                animator.SetBool(isClosed,false);
                animator.SetTrigger(isOpened);
                isDoorOpen = true;
                //Debug.Log("Door closed");
        }
    }
    public new void CanInteractVisual()
    {
        base.CanInteractVisual();
    } 
}
