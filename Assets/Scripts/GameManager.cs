using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public ISingleton[] singletons;

    // The original start that controls all other Inits
    void Start() {
        Init();

    }
    public override void Init() {

        singletons = new ISingleton[10] {
            CameraController._instance,
             GridManager._instance,
             PlayerManager._instance,
             GodmodeScript._instance,
             CraftingManager._instance,
             InventoryUIManager._instance,
             UIManager._instance,
             InputManager._instance,
              PlayerStats._instance,
             ConsumeEffectHandler._instance

        };

        foreach (ISingleton singleton in singletons) {
            if (singleton != null) {
                singleton.Init();
            }
        }
    }
}
