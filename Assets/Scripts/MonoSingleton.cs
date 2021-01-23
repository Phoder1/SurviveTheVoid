using UnityEngine;
public interface ISingleton
{
    void SoftReset();
    void HardReset();
    void Init();
}
public abstract class MonoSingleton<T> : MonoBehaviour,ISingleton where T : Component
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
    /// <summary>
    /// run at start
    /// </summary>
    public abstract void Init();


    /// <summary>
    /// hard reset runs when the game starts?
    /// </summary>

    public abstract void HardReset();
    /// <summary>
    ///  soft reset runs when the players die
    /// </summary>
    public abstract void SoftReset();
}
