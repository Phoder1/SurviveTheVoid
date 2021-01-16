using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    private Camera cameraComp;
    private GridManager gridManager;
    private Vector2 GetCameraRealSize => new Vector2(GetCameraComp.orthographicSize * 2 * GetCameraComp.aspect, GetCameraComp.orthographicSize * 2);
    private Vector3 GetCamCornerPosition => new Vector3(transform.position.x, transform.position.y, 0) - (Vector3)GetCameraRealSize / 2;
    private Rect GetWorldView => new Rect(_instance.GetCamCornerPosition, _instance.GetCameraRealSize);
    StarsParallaxController starsCont ;
    public Camera GetCameraComp {
        get {
            if (cameraComp == null)
                cameraComp = Camera.main;
            return cameraComp;
        }
    }

    public override void Init() {
        gridManager = GridManager._instance;
        starsCont = GetComponent<StarsParallaxController>();

    }
    public void ZoomOut(float amount) {
        cameraComp.orthographicSize += amount;
        starsCont.UpdateViewSize();
    }
    public void UpdateView() {
        gridManager.UpdateView(GetWorldView);
    }
}
