
public class GameManager : MonoSingleton<GameManager>
{
    public ISingleton[] singletons;

    // The original start that controls all other Inits
    void Start()
    {
        Init();

    }
    public override void Init()
    {
        singletons = new ISingleton[11] {
            CameraController._instance,  // alon
             GridManager._instance, // alon
             UIManager._instance, // -----
             PlayerStats._instance, // alon
             InputManager._instance, // rei - V
             PlayerManager._instance, // rei - V
             GodmodeScript._instance, //-----
             CraftingManager._instance, // elor
             InventoryUIManager._instance, // elor
             EffectHandler._instance, // alon
             ConsumeablesHandler._instance //

        };

        foreach (ISingleton singleton in singletons)
        {
            if (singleton != null)
            {
                singleton.Init();
            }
        }
    }


    public void DeathReset()
    {
        
    }
}
