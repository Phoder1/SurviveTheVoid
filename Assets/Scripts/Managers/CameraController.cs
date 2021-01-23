using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    [SerializeField] private Camera zoomedOutCameraComp;
    [SerializeField] private Camera zoomedInCameraComp;

    private GridManager gridManager;
    private Vector2 GetCameraRealSize => new Vector2(CurrentActiveCamera.orthographicSize * 2 * CurrentActiveCamera.aspect, CurrentActiveCamera.orthographicSize * 2);
    private Vector3 GetCamCornerPosition => new Vector3(transform.position.x, transform.position.y, 0) - (Vector3)GetCameraRealSize / 2;
    private Rect GetWorldView => new Rect(_instance.GetCamCornerPosition, _instance.GetCameraRealSize);


    private StarsParallaxController starsCont;
    private Camera CurrentActiveCamera;
    private bool isZoomedIn = false;
    public bool GetSetIsZoomedIn { 
        get => isZoomedIn;
        set { 
            if(isZoomedIn != value) {
                isZoomedIn = value;
                SetZoomIn(value);
            }
        } 
    }


    public override void Init() {
        gridManager = GridManager._instance;
        starsCont = GetComponent<StarsParallaxController>();
        CurrentActiveCamera = zoomedOutCameraComp;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            GetSetIsZoomedIn = !GetSetIsZoomedIn;
        }
    }
    private void SetZoomIn(bool isZoomIn) {

        zoomedOutCameraComp.enabled = !isZoomIn;
        zoomedInCameraComp.enabled = isZoomIn;
        CurrentActiveCamera = (isZoomIn ? zoomedInCameraComp : zoomedOutCameraComp);
        starsCont.UpdateViewSize();
        UpdateView();
    }
    public void UpdateView() {
        gridManager.UpdateView(GetWorldView);
    }
}
