using UnityEngine;

public class StarsParallaxController : MonoBehaviour
{
    CameraController cameraController;


    [SerializeField] private StarsParallax[] starsParallax;

    private void Start() {
        cameraController = CameraController._instance;
    }


    private void Update() {
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.rotation *= Quaternion.Euler(new Vector3(0,0,stars.speed));
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
