using UnityEngine;
public interface ISingleton
{

    void Init();
}
public abstract class MonoSingleton<T> : MonoBehaviour,ISingleton where T : Component
{
    public static T _instance;

    public virtual void Awake() {
        if (isActiveAndEnabled) {
            if (_instance == null) {
                    _instance = this as T;
            }
            else if(_instance != this as T){
                Destroy(this);
            }
        }
    }
    /// <summary>
    /// run at start
    /// </summary>
    public abstract void Init();


}
