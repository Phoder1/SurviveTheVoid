using UnityEngine;


[CreateAssetMenu(fileName = "New Resource Pack", menuName = "Crafting/" + "Resource Pack")]
public class ItemPackSO : ScriptableObject
{
    [SerializeField]
    private ItemSO[] itemsArr;
    public ItemSO[] getitemsArr { get => itemsArr; }


    [ContextMenu("Assign ID by Order")]
    public void AssignIDByOrder()
    {
        for (int i = 0; i < itemsArr.Length; i++)
        {
            itemsArr[i].itemID = i;
        }
        Debug.Log("All Items Got ID");
    }











}

