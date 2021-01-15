using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private ISingleton[] singletons;

    // The original start that controls all other Inits
    void Start() {
        Init();

    }
    public override void Init() {
        singletons = new ISingleton[7] {
             CameraScript._instance,
             GridManager._instance,
             InputManager._instance,
             PlayerManager._instance,
             CraftingManager._instance,
             InventoryUIManager._instance,
             UIManager._instance
             
        };

        foreach (ISingleton singleton in singletons) {
            if(singleton != null) {
                singleton.Init();
            }
        }
    }


}
