using System.Collections;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] float distanceFromCenter;
    [Min(0.001f)]
    [SerializeField] float circularSpeed;
    [Min(0.001f)]
    [SerializeField] float offsetSpeed;
    [SerializeField] float delay;
    float zPos;
    CameraController cameraController;
    float offset;
    bool moving = false;
    float startTime;
    private void Start() {
        cameraController = CameraController._instance;
        zPos = transform.position.z;
        cameraController.UpdateView();
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > delay) {
            float time = Time.time - delay;
            Debug.Log(time);
            transform.position = new Vector3(Mathf.Sin(time * circularSpeed) * (distanceFromCenter / 2 + offset), (Mathf.Cos(time * circularSpeed) * (distanceFromCenter / 2 + offset)), zPos);
            cameraController.UpdateView();
            offset = -Mathf.Cos(time * offsetSpeed) * distanceFromCenter / 2;
        }
    }
}
