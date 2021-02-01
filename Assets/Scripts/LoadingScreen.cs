using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(GotoScene());
    }
    IEnumerator GotoScene() {
        Scene mainMenu = SceneManager.GetSceneByBuildIndex(0);
        if (mainMenu.isLoaded) {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(mainMenu);

            // Wait until the asynchronous scene fully loads
            if (asyncUnload != null) {
                while (!asyncUnload.isDone) {
                    yield return null;
                }
            }
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
        Debug.Log("loaded main screen");

    }
}
