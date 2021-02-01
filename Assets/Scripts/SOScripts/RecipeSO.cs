using UnityEngine;
public enum SectionEnum
{
    Blocks,
    Furnitures,
    Plants,
    Weapons,
    Tools,
    Food,
    Generic,
    Gear
}

public enum ProcessorType
{
    CraftingTable,
    Cooker,
    Furnace
}


[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/" + "Recipe")]
public class RecipeSO : ScriptableObject
{

    [SerializeField]
    private ItemSlot[] ItemCostArr;
    public ItemSlot[] getitemCostArr { get => ItemCostArr; }

    [SerializeField]
    private ItemSlot outcomeItem;
    public ItemSlot getoutcomeItem { get => outcomeItem; }

    [SerializeField]
    private int tier;
    public int getTier { get => tier; }

    [SerializeField]
    private SectionEnum section;
    public SectionEnum getSection { get => section; }

    [SerializeField]
    private bool isUnlocked;
    public bool getisUnlocked { get => isUnlocked; }

    [SerializeField]
    ProcessorType processorType;
    public ProcessorType GetProcessorType => processorType;

    // 60 = 1 minute
    [SerializeField]
    private int CraftingTime;
    public int GetCraftingTime => CraftingTime;
    [SerializeField] float expReward;
    public float GetExpReward => expReward;
    public void UpdateIfRecipeUnlocked(bool _unlocked)
    {
        isUnlocked = _unlocked;
    }







    //For Multiple crafting only for the amount holder in the crafting manager!!!!

    public void UpdateAmountHolder(ItemSlot[] itemCostArr,ItemSlot OutCome,int craftingTime)
    {
        int TempTime = craftingTime;


        //seperating scriptable object 
        ItemSO OutComeitemso = OutCome.item;
        int OutComeAmount = OutCome.amount;
        ItemSlot TempSlot = new ItemSlot(OutComeitemso, OutComeAmount);

        ItemSO[] Costitemso = new ItemSO[itemCostArr.Length];
        int[] CostAmount = new int[itemCostArr.Length];


        
        for (int i = 0; i < itemCostArr.Length; i++)
        {
            Costitemso[i] = itemCostArr[i].item;
            CostAmount[i] = itemCostArr[i].amount;
        }

        ItemSlot[] TempArr = new ItemSlot[itemCostArr.Length];

        for (int i = 0; i < TempArr.Length; i++)
        {
            TempArr[i] = new ItemSlot(Costitemso[i], CostAmount[i]);
        }


      

        ItemCostArr = TempArr;
        outcomeItem = TempSlot;
        CraftingTime = TempTime;

    } 

    public void DoubleAmountOutCome(int Double)
    {
        outcomeItem.amount *= Double;
        CraftingTime *= Double;
        for (int i = 0; i < ItemCostArr.Length; i++)
        {
            ItemCostArr[i].amount *= Double;
        }
        Debug.Log("Testing Doubler: Outcome Amount: " + outcomeItem.amount + " Crafting Time: " + CraftingTime + " Amount: " + ItemCostArr[0].amount);
    }

   



}

