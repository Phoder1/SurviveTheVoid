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

    private void OnDisable() {
        foreach (StarsParallax stars in starsParallax) {
            stars.mr.sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
        }
    }


    private void Update() {
        foreach (StarsParallax stars in starsParallax) {
            stars.offset += new Vector2(stars.speed * Time.deltaTime, 0f);

            stars.mr.sharedMaterial.SetTextureOffset("_MainTex", stars.offset);
        }
    }

    public void UpdateViewSize() {
        foreach (StarsParallax stars in starsParallax) {
            stars.mr.transform.localScale *= mainCamera.orthographicSize / cameraSize;
        }
        cameraSize = mainCamera.orthographicSize;
    }


    [System.Serializable]
    private class StarsParallax
    {
        [SerializeField] internal MeshRenderer mr;
        [Range(0.001f, 0.01f)]
        [SerializeField] internal float speed;
        internal Vector2 offset = Vector2.zero;
    }
}
