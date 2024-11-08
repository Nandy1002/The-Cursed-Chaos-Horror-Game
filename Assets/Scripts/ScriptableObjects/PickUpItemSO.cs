using UnityEngine;

[CreateAssetMenu(fileName = "PickUpItemSO", menuName = "PickUp Items Scriptable Object")]
public class PickUpItemSO : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
    public PickUpItem itemScript;
}
