using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public enum ItemName
    {
        Flower,
        OakLog,
        CraftingTable,
        WoodenStick,
        WoodenSword,
        WoodenHoe,
        Apple,
        WoodBlock,
        Stone,
        StoneBlock
    }



    public enum ItemType
    {
        Generic,

        Consumable,
        Building,
        Equipable

    }





    [CreateAssetMenu(fileName = "New Generic", menuName = "Items/" + "Generic")]
    public class ItemSO : ScriptableObject
    {
        [SerializeField]
        private int maxStackSize;
        public int getmaxStackSize => maxStackSize;

        [SerializeField]
        private string description;
        public string getDescription => description;

        [SerializeField]
        private ItemName itemEnum;
        public ItemName getItemEnum => itemEnum;

        [SerializeField]
        private Sprite sprite;
        public Sprite getsprite { get => sprite; }

    [SerializeField]
    private ItemType itemType;

    public ItemType GetItemType { get => itemType; }
}


