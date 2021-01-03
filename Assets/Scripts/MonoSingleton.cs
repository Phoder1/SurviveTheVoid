using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    public static T _instance;

    public virtual void Awake() {
        if (isActiveAndEnabled) {
            if (_instance == null) {
                    _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if(_instance != this as T){
                Destroy(this);
            }
        }
    }

    //public abstract void Init();
}
