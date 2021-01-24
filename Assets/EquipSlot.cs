using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public Image SlotImage;
    public int ChestId;
    public int EquipId;

    public bool IsDraggingThis;



    public void OnPointerDown(PointerEventData eventData)
    { 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!IsDraggingThis)
        DeHighLightEquipSlot();

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDraggingThis)
      

        IsDraggingThis = false;
        DeHighLightEquipSlot();










    }

    public void ShowEquippedGear(ItemSlot Equip)
    {
        SlotImage.gameObject.SetActive(true);
        SlotImage.sprite = Equip.item.getsprite;
        DeHighLightEquipSlot();
    }

    public void NoGearEquipped()
    {
        SlotImage.gameObject.SetActive(false);
        SlotImage.sprite = null;
    }


    public void HighLightEquipSlot()
    {
        transform.GetComponent<Image>().color = Color.yellow;

    }

    public void DeHighLightEquipSlot()
    {
        transform.GetComponent<Image>().color = Color.white;

    }




}
