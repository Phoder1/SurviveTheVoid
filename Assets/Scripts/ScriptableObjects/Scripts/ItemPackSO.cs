using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "New Resource Pack", menuName = "Crafting/" + "Resource Pack")]
    public class ItemPackSO : ScriptableObject
    {
        [SerializeField]
        private ItemSO[] itemsArr;
        public ItemSO[] getitemsArr { get => itemsArr; }

    }

