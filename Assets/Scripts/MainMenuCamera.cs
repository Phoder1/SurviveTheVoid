﻿using System.Collections;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] float distanceFromCenter;
    [Min(0.001f)]
    [SerializeField] float circularSpeed;
    [Min(0.001f)]
    [SerializeField] float offsetSpeed;
    [SerializeField] float startDelay;
    float zPos;
    CameraController cameraController;
    float offset;
    bool travel=false;
    private void Start() {
        cameraController = CameraController._instance;
        zPos = transform.position.z;
        
        StartCoroutine(EnableTravel(startDelay));
    }
    // Update is called once per frame
    void Update()
    {
        float time = Time.time - startDelay;
        if (travel) {
        offset = - Mathf.Cos(time * offsetSpeed) * distanceFromCenter/2;
        transform.position = new Vector3(Mathf.Sin(time * circularSpeed) * (distanceFromCenter/2 + offset), (Mathf.Cos(time * circularSpeed) * (distanceFromCenter/2 + offset)) , zPos);
        cameraController.UpdateView();
        }
    }
    private IEnumerator EnableTravel(float startDelay) {
        yield return new WaitForSeconds(startDelay);
        travel = true;
    }
}
