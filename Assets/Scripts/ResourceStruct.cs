using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStruct
{
    public int id;
    public string name;
     ResourceStruct(int _id, string _name)
    {
        id = _id;
        name = _name;
    }

    public static ResourceStruct EXAMPLE { get { return new ResourceStruct(1, "EXAMPLE"); } }
}
