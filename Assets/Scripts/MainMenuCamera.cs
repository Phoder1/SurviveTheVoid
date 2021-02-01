using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] float distanceFromCenter;
    [Min(0.001f)]
    [SerializeField] float circularSpeed;
    [Min(0.001f)]
    [SerializeField] float offsetSpeed;
    float zPos;
    CameraController cameraController;
    float offset;
    private void Start() {
        cameraController = CameraController._instance;
        zPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Sin(Time.time * circularSpeed) * (distanceFromCenter/2 + offset), (Mathf.Cos(Time.time  * circularSpeed) * (distanceFromCenter/2 + offset)) , zPos);
        cameraController.UpdateView();
        offset = - Mathf.Cos(Time.time * offsetSpeed) * distanceFromCenter/2;
    }
}
