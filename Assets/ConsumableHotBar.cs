using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableHotBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public int ChestId;
    public int SlotKey;
    //public ItemSlot ItemHolder;
    public Image ItemBarImage;
    public TextMeshProUGUI ItemBarText;
    public bool IsDraggingThis;


    public void OnPointerDown(PointerEventData eventData)
    {
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
     




    }

    public void OnPointerExit(PointerEventData eventData)
    {
      
    }

    public void OnPointerUp(PointerEventData eventData)
    {

      


    }

    void ConsumeHotBar()
    {
        var checkIfSlotIsItem = Inventory.GetInstance.GetItemFromInventoryButton(ChestId, SlotKey);


        if (checkIfSlotIsItem == null || checkIfSlotIsItem.item == null)
            return;


        ItemSlot itemCache = new ItemSlot(checkIfSlotIsItem.item, 1);

        if (itemCache.item.GetItemType == ItemType.Consumable)
        {
            if (ConsumeablesHandler._instance.GetEffectCoolDown(itemCache.item as ConsumableItemSO))
            {
                if (Inventory.GetInstance.RemoveItemFromInventory(ChestId, new ItemSlot(itemCache.item, 1)))
                {
                    Debug.Log("Consumed: " + itemCache.item.getItemName);
                    (itemCache.item as ConsumableItemSO).ApplyEffect();
                }
            }

        }
        InventoryUIManager._instance.UpdateHotKeysToUI();
    }




    public void ShowEquippedConsumable(ItemSlot Item)
    {
        ItemBarImage.sprite = Item.item.getsprite;
        ItemBarText.text = Item.amount.ToString();
        GameObject ItemImage = transform.GetChild(0).gameObject;
        ItemImage.SetActive(true);
        DeHightLightHotKey();
    }
    public void NoConsumableEquipped()
    {
        GameObject ItemImage = transform.GetChild(0).gameObject;
        ItemImage.SetActive(false);
        ItemBarImage.sprite = null;
        ItemBarText.text = "";
    }


    public void HighLightHotKey()
    {
        this.gameObject.GetComponent<Image>().color = Color.yellow;
    }
    public void DeHightLightHotKey()
    {
        this.gameObject.GetComponent<Image>().color = Color.white;
    }


}
