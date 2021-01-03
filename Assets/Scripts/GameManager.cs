using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    GridManager gridManager;
    PlayerManager playerManager;
    CameraScript cameraScript;
    CraftingManager craftingManager;
    // Start is called before the first frame update
    void Start()
    {
        gridManager = GridManager._instance;
        playerManager = PlayerManager._instance;
        cameraScript = CameraScript._instance;
        craftingManager = CraftingManager._instance;
        //playerManager Init must run before gridManager
        if (cameraScript != null)
            cameraScript.Init();
        if(playerManager != null) 
            playerManager.Init();
        if (gridManager != null)
            gridManager.Init();
        //if (craftingManager != null)
        //    craftingManager.Init();
        
    }

    // Update is called once per frame
    void Update()
    {
      
        //Debug.Log("hi");
    }
}
