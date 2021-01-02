using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T GetInstance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    public virtual void Awake() {
        if (isActiveAndEnabled) {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if(_instance != this as T){
                Destroy(gameObject);
            }
        }
    }
}
