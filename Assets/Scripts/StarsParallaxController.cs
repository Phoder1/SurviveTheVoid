using UnityEngine;

public class StarsParallaxController : MonoBehaviour
{

    [SerializeField] private StarsParallax[] starsParallax;


    private void Update() {
        foreach (StarsParallax stars in starsParallax) {
            stars.gameobject.transform.rotation *= Quaternion.Euler(new Vector3(0,0,stars.speed));
        }
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
