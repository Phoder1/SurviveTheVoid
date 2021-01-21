using UnityEngine;

public class StarsParallaxController : MonoBehaviour
{
    //private Vector2 cameraRealSize => new Vector2(mainCamera.orthographicSize * 2 * mainCamera.aspect, mainCamera.orthographicSize * 2);
    private Camera mainCamera;
    private float cameraSize;

    [SerializeField] private StarsParallax[] starsParallax;

    private void Start() {
        mainCamera = Camera.main;
        cameraSize = mainCamera.orthographicSize;
    }


    private void Update() {
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.rotation *= Quaternion.Euler(new Vector3(0,0,stars.speed));
        }
    }

    public void UpdateViewSize() {
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.localScale *= mainCamera.orthographicSize / cameraSize;
        }
        cameraSize = mainCamera.orthographicSize;
    }


    [System.Serializable]
    private class StarsParallax
    {
        [SerializeField] internal GameObject gameobject;
        [SerializeField] internal float speed;
        internal Vector2 offset = Vector2.zero;
    }
}
