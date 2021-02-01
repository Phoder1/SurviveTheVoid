using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    [SerializeField] private GameObject zoomedOutCameraObj;
    [SerializeField] private GameObject zoomedInCameraObj;
    [SerializeField] private StarsParallax[] starsParallax;

    private Camera zoomedOutCamera;
    private Camera zoomedInCamera;

    private GridManager gridManager;
    private Vector2 GetCameraRealSize => new Vector2(zoomedOutCamera.orthographicSize * 2 * zoomedOutCamera.aspect, zoomedOutCamera.orthographicSize * 2);
    private Vector3 GetCamCornerPosition => new Vector3(transform.position.x, transform.position.y, 0) - (Vector3)GetCameraRealSize / 2;
    private Rect GetWorldView => new Rect(_instance.GetCamCornerPosition, _instance.GetCameraRealSize);
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
        zoomedInCamera = zoomedInCameraObj.GetComponent<Camera>();
        zoomedInCameraObj.SetActive(false);
        zoomedOutCamera = zoomedOutCameraObj.GetComponent<Camera>();
        zoomedOutCameraObj.SetActive(true);
        currentActiveCamera = zoomedOutCamera;
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.GetComponent<SpriteRenderer>().size = new Vector2(GetCameraRealSize.x * 1.2f, GetCameraRealSize.x * 1.2f);
        }
        UpdateView();
        gridManager.GenerateStartingArea();
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            GetSetIsZoomedIn = !GetSetIsZoomedIn;
        }
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, stars.speed));
        }
    }
    private void SetZoomIn(bool isZoomIn) {

        zoomedOutCameraObj.SetActive(!isZoomIn);
        zoomedInCameraObj.SetActive(isZoomIn);
        currentActiveCamera = (isZoomIn ? zoomedInCamera : zoomedOutCamera);
        if (isZoomIn)
            UpdateViewSize(zoomedOutCamera.orthographicSize, zoomedInCamera.orthographicSize);
        else
            UpdateViewSize(zoomedInCamera.orthographicSize, zoomedOutCamera.orthographicSize);
    }
    public void UpdateView() {
        gridManager.UpdateView(GetWorldView);
    }

    public void UpdateViewSize(float oldCameraScalem, float newCameraScale) {
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.localScale *= newCameraScale / oldCameraScalem;
        }
    }


    [System.Serializable]
    private class StarsParallax
    {
        [SerializeField] internal GameObject gameobject;
        [SerializeField] internal float speed;
        internal Vector2 offset = Vector2.zero;
    }
}
