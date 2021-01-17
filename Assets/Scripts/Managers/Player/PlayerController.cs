using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    CameraController cameraController;
    GridManager gridManager;
    public override void Init() {
        cameraController = CameraController._instance;
        gridManager = GridManager._instance;
    }
    public void Move(Vector2 moveVector) {
        Vector2 currentPos = transform.position;
        Vector2 nextPos = currentPos + moveVector;
        if (gridManager.IsTileWalkable(nextPos, moveVector) || Input.GetKey(KeyCode.LeftShift)) {

            transform.Translate(moveVector);
            UpdateView();
        }
    }

    private void UpdateView() => cameraController.UpdateView();
}
