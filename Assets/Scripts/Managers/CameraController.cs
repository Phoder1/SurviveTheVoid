using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    [SerializeField] private GameObject zoomedOutCameraObj;
    [SerializeField] private GameObject zoomedInCameraObj;

    private Camera zoomedOutCamera;
    private Camera zoomedInCamera;

    private GridManager gridManager;
    private Vector2 GetCameraRealSize => new Vector2(zoomedOutCamera.orthographicSize * 2 * zoomedOutCamera.aspect, zoomedOutCamera.orthographicSize * 2);
    private Vector3 GetCamCornerPosition => new Vector3(transform.position.x, transform.position.y, 0) - (Vector3)GetCameraRealSize / 2;
    private Rect GetWorldView => new Rect(_instance.GetCamCornerPosition, _instance.GetCameraRealSize);


    private StarsParallaxController starsCont;
    private Camera currentActiveCamera;
    public Camera GetCurrentActiveCamera => currentActiveCamera;
    private bool isZoomedIn;
    public bool GetSetIsZoomedIn
    {
        get => isZoomedIn;
        set
        {
            if (isZoomedIn != value)
            {
                isZoomedIn = value;
                SetZoomIn(value);
            }

        }
    }


    public override void Init() {
        gridManager = GridManager._instance;
        starsCont = GetComponent<StarsParallaxController>();
        zoomedInCamera = zoomedInCameraObj.GetComponent<Camera>();
        zoomedOutCamera = zoomedOutCameraObj.GetComponent<Camera>();

        SetZoomIn(false);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            GetSetIsZoomedIn = !GetSetIsZoomedIn;
        }
    }
    private void SetZoomIn(bool isZoomIn) {

        zoomedOutCameraObj.SetActive(!isZoomIn);
        zoomedInCameraObj.SetActive(isZoomIn);
        Camera previousCamera = GetCurrentActiveCamera;
        currentActiveCamera = (isZoomIn ? zoomedInCamera : zoomedOutCamera);
        if (isZoomIn)
            starsCont.UpdateViewSize(zoomedOutCamera.orthographicSize, zoomedInCamera.orthographicSize);
        else
            starsCont.UpdateViewSize(zoomedInCamera.orthographicSize, zoomedOutCamera.orthographicSize);
    }
    public void UpdateView() {
        gridManager.UpdateView(GetWorldView);
    }
}
