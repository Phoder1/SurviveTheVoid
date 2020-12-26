﻿using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public ItemSO resource;
    public int amount;

    public ItemSlot(ItemSO resource, int amount) {
        this.resource = resource;
        this.amount = amount;
    }
}






