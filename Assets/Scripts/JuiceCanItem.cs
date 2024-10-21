using UnityEngine;

public class JuiceCanItem : PickUpItem 
{
    private void Awake(){
        canMultiPick = false;
        timesOfUse = 5;
    }
    
}
