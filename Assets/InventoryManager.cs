using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager _instance;

    [Header("Equip Related")]
    // 0 = head, 1 = Chest, 2 = Legging, 3 = Gloves, 4 = Shoes
    public Image[] EquipSlots;
    public GearItemSO[] EquippedSlots;


    [Header("Inventory Related")]
    public Image[] InventorySlots;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            UpdateInventoryToUI();
        }
    }


    void UpdateInventoryToUI()
    {
        for (int i = 0; i < inventory.GetInventory.Length; i++)
        {
            if (inventory.GetInventory[i] != null)
            {
                if(!InventorySlots[i].isActiveAndEnabled)
                    SetInventorySlotActive(i, true);
                InventorySlots[i].sprite = inventory.GetInventory[i].item.getsprite;
            }
            else
            {
                if (InventorySlots[i].isActiveAndEnabled)
                    SetInventorySlotActive(i, false);
                InventorySlots[i].sprite = null;
            }
        }
    }

    void SetInventorySlotActive(int Index,bool IsActive)
    {
        InventorySlots[Index].gameObject.SetActive(IsActive);
    }



}
