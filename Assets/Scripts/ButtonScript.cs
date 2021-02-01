using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    private void OnDisable()
    {
        UIManager._instance.ButtonPressedUp();
    }
}
