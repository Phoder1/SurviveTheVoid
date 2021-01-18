using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CheatMenu : MonoBehaviour
{
    Inventory inventory;
    [SerializeField] bool isCheatOn;
    [SerializeField] GameObject CheatMenuBackGround;
    [SerializeField] ItemPackSO ItemPack;
    [SerializeField] GameObject ItemSlotPrefab;
    [SerializeField] GameObject Content;
    // Update is called once per frame

    private void Start()
    {
        if (isCheatOn)
        {
            inventory = Inventory.GetInstance;
            InstantiateItemSlots();
        }
        else
        {
            Destroy(CheatMenuBackGround);
            Destroy(this);
        }
    }

    void Update()
    {
        if (isCheatOn)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                ToggleCheatMenuUI();
            }
        }
    }

    void ToggleCheatMenuUI()
    {
        CheatMenuBackGround.SetActive(!CheatMenuBackGround.activeInHierarchy);
        //if (CheatMenuBackGround.activeInHierarchy)
        //{

        //}
    }

    void InstantiateItemSlots()
    {
        for (int i = 0; i < ItemPack.getitemsArr.Length; i++)
        {
           
            GameObject instiatedSlot = Instantiate(ItemSlotPrefab, Content.transform);
            //instiatedSlot.GetComponent<Image>().color = new Color(190, 99, 22);
            instiatedSlot.transform.GetChild(0).GetComponent<Image>().sprite = ItemPack.getitemsArr[i].getsprite;
            instiatedSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ItemPack.getitemsArr[i].getItemName;
           
            // add function to the buttons

            //YourButton.onClick.AddListener(() => TaskOnClick());
            ItemSlot TempSlot = new ItemSlot(ItemPack.getitemsArr[i], ItemPack.getitemsArr[i].getmaxStackSize);
            instiatedSlot.GetComponent<Button>().onClick.AddListener(() => inventory.AddToInventory(0, TempSlot));
            instiatedSlot.GetComponent<Button>().onClick.AddListener(() => UpdateCraftReqMats());


        }
    }

    void UpdateCraftReqMats()
    {
        if(CraftingManager._instance.selectedRecipe != null)
        {
            CraftingManager._instance.ShowRecipe(CraftingManager._instance.selectedRecipe);
            Debug.Log("Updating Mats from cheat menu");
        }
    }











}
