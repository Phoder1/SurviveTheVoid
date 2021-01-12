using UnityEngine;

public class BuildingState : StateBase
{
    public override void ButtonA()
    {
        Debug.Log("BuildingState");
        InputManager._instance.PressedConfirmBuildingButton();
    }


    public override void ButtonB()
    {
        PlayerStateMachine.GetInstance.SwitchState(InputState.RemovalState);
    }

}
