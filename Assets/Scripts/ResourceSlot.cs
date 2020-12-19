using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSlot
{
    public ResourceStruct resource;
    public int amount;
    public Sprite sprite;
    public ResourceSlot(ResourceStruct resource, int amount ) {
        this.resource = resource;
        this.amount = amount;
    }

  
}
