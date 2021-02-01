
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public ISingleton[] singletons;
    public static event Action DeathEvent;
    public static event Action RespawnEvent;
    // The original start that controls all other Inits
    void Start()
    {
        Debug.Log("Gamemanager Start!");
        Init();

    }
    public override void Init()
    {
        singletons = new ISingleton[12] {
             GridManager._instance, // alon
            CameraController._instance,  // alon
             UIManager._instance, // -----
             PlayerStats._instance, // alon
             InputManager._instance, // rei - V
             PlayerManager._instance, // rei - V
             GodmodeScript._instance, //-----
             CraftingManager._instance, // elor
             InventoryUIManager._instance, // elor
             EffectHandler._instance, // alon
             ConsumeablesHandler._instance, //
             SoundManager._instance

        };

        foreach (ISingleton singleton in singletons)
        {
            if (singleton != null)
            {
                singleton.Init();
            }
        }
    }
    static bool startedDeathEvent = false;
    public static void OnDeath()
    {
        if (!startedDeathEvent)
        {
            startedDeathEvent = true; 
            _instance.StartCoroutine(_instance.DeathTransition());
        }
        Debug.Log("Died!");
    }
    const float extraDelay = 1f;
    public IEnumerator DeathTransition()
    {
        DeathEvent?.Invoke();
        //  Debug.Log("Start");
        yield return new WaitForSeconds(PlayerManager._instance.GetPlayerGFX.GetDeathAnimLength);

        // Debug.Log("Player Died");
        UIManager._instance.BlackPanel(true);

        yield return new WaitForSeconds(extraDelay);
        UIManager._instance.BlackPanel(false);
        RespawnEvent?.Invoke();
        startedDeathEvent = false ;
    }
}
