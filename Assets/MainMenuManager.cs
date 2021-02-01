using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]Canvas SettingMenu;

    public void playButton()
    {

        StartCoroutine(LoadYourAsyncScene(SceneManager.GetActiveScene().buildIndex + 1));
    }
    public void SettingButton()
    {
        SettingMenu.enabled = true;
    }
    public void CloseSettings()
    {
        SettingMenu.enabled = false;
    }
    public void ExitButton()
    {
        Application.Quit();
    }

    IEnumerator LoadYourAsyncScene(int sceneNum) {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
        Debug.Log("loaded load screen");
    }
}
