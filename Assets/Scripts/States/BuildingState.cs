using UnityEngine;

public class BuildingState : StateBase
{
    public override void ButtonB()
    {
        Debug.Log("BuildingState");
        InputManager._instance.PressedConfirmBuildingButton();

    }

}
