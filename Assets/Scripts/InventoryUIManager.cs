using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{
    [Header("Equip Related")]
    // 0 = head, 1 = Chest, 2 = Legging, 3 = Gloves, 4 = Shoes
    public Image[] EquipSlots;
    public GearItemSO[] EquippedSlots;
    [SerializeField] bool IsInventoryOn = true;

    [Header("Inventory Related")]
    
    public GameObject[] InventorySlots;
    Image[] InventorySlotImage;
    TextMeshProUGUI[] inventorySlotText;

    Inventory inventory;


    public void OnPressedInventoryButton(int buttonId)
	{
        ItemSlot itemCache = inventory.GetItemFromInventoryButton(0, buttonId);

		if (itemCache.item.GetItemType == ItemType.Building)
		{
            PlayerStateMachine.GetInstance.SwitchState(InputState.BuildState);
            //InputManager._instance.SetBuildingTile(itemCache);
		}
	}




    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        UpdateInventory();
    }

    //public boolean = setactive of all inventory,  only update inventory after this boolean is true, if false dont update

    public void UpdateInventory()
    {
        if (IsInventoryOn)
        {
            UpdateInventoryToUI();
        }
    }

    public override void Init()
    {
        inventory = Inventory.GetInstance;
    }



    public void UpdateInventoryToUI()
    {
        InventorySlotImage = new Image[inventory.GetInventory.Length];
        inventorySlotText = new TextMeshProUGUI[inventory.GetInventory.Length];
        for (int i = 0; i < inventory.GetInventory.Length; i++)
        {
            InventorySlotImage[i] = InventorySlots[i].transform.GetChild(0).GetComponent<Image>();

            inventorySlotText[i] = InventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (inventory.GetInventory[i] != null)
            {
                if (!InventorySlotImage[i].isActiveAndEnabled)
                {
                    SetInventorySlotActive(i, true);
                }

                InventorySlotImage[i].sprite = inventory.GetInventory[i].item.getsprite;

               UpdateInventorySlotAmountUI(i, inventory.GetInventory[i].amount,true);
            }
            else
            {
                if (InventorySlotImage[i].isActiveAndEnabled)
                {
                    SetInventorySlotActive(i, false);
                }
                InventorySlotImage[i].sprite = null;
                UpdateInventorySlotAmountUI(i, 0, false);


            }
        }
    }

    void SetInventorySlotActive(int Index,bool IsActive)
    {
        InventorySlotImage[Index].gameObject.SetActive(IsActive);
        inventorySlotText[Index].gameObject.SetActive(IsActive);
    }
    void UpdateInventorySlotAmountUI(int Index,int Amount,bool IsItemExist)
    {
        if (IsItemExist && Amount > 1)
        {
            inventorySlotText[Index].text = Amount.ToString();
        }
        else
        {
            inventorySlotText[Index].text = "";
        }

    }


}
