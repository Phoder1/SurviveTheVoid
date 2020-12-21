using System;
using TMPro;
using UnityEngine;

public class FPScounter : MonoBehaviour {
    [SerializeField]
    TextMeshProUGUI fpsText;
    [SerializeField]
    int updateInterval = 6;

    int[] fpsHistoryArr;

    int currentFrameCount;
    private void Start() {
        fpsHistoryArr = new int[updateInterval];
        currentFrameCount = updateInterval;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 150;
    }
    // Update is called once per frame
    void Update() {
        if (currentFrameCount > 0) {
            fpsHistoryArr[currentFrameCount - 1] = Mathf.RoundToInt(1 / Time.deltaTime);
            currentFrameCount--;
        }
        else {
            int sum = Mathf.RoundToInt(1 / Time.unscaledDeltaTime);
            Array.ForEach(fpsHistoryArr, i => sum += i);
            fpsText.text = Mathf.RoundToInt(sum / (updateInterval + 1)).ToString();
            currentFrameCount = updateInterval;
        }

    }
}
