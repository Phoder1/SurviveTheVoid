using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSlot
{
    public ResourceStruct resource;
    public int amount;
    public Sprite sprite;
    public ResourceSlot(ResourceStruct resource, int amount)
    {
        this.resource = resource;

        if (resource.maxStackSize == 1)
            this.amount = 1;
   
    }
}
