using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    GridManager gridManager;
    PlayerManager playerManager;
    CameraScript cameraScript;


    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gridManager = GridManager._instance;
        playerManager = PlayerManager._instance;
        cameraScript = CameraScript._instance;

        //playerManager Init must run before gridManager
        if (cameraScript != null)
            cameraScript.Init();
        if(playerManager != null) 
            playerManager.Init();
        if (gridManager != null)
            gridManager.Init();
        
    }

    // Update is called once per frame
    void Update()
    {
      
        //Debug.Log("hi");
    }
}
