using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager _instance;

    [Header("Equip Related")]
    // 0 = head, 1 = Chest, 2 = Legging, 3 = Gloves, 4 = Shoes
    public Image[] EquipSlots;
    public GearItemSO[] EquippedSlots;
    [SerializeField] bool IsInventoryOn = true;

    [Header("Inventory Related")]
    
    public GameObject[] InventorySlots;
    [SerializeField]Image[] InventorySlotImage;
    [SerializeField] TextMeshProUGUI[] inventorySlotText;

    Inventory inventory;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        inventory = Inventory.GetInstance;
    }









    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < EquippedSlots.Length; i++)
        //{
        //    if(EquippedSlots[i] == null)
        //    {
        //        if (EquipSlots[i].gameObject.activeInHierarchy)
        //        {
        //            EquipSlots[i].gameObject.SetActive(false);
        //        }

        //    }
        //    else
        //    {
        //        if (!EquipSlots[i].gameObject.activeInHierarchy)
        //        {
        //            EquipSlots[i].gameObject.SetActive(true);
        //        }
        //        EquipSlots[i].sprite = EquippedSlots[i].getsprite;
        //    }
        //}

    }

    // Update is called once per frame
    void Update()
    {

        if (IsInventoryOn)
        {
            UpdateInventoryToUI();
        }
    }

    //public boolean = setactive of all inventory,  only update inventory after this boolean is true, if false dont update
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
