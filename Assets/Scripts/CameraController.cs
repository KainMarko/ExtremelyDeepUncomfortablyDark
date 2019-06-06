using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private float distance = 6.0f;
    private float height = 5.0f;

    private float rotateSpeed = 2.0f;

    private const float distance_max = 12.0f;
    private const float distance_min = 3.0f;

    private const float height_max = 8.0f;
    private const float height_min = 3.5f;
    public float currentX = 0.0f;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;
    }

    public void Update()
    {
        currentX += Input.GetAxis("Horizontal Rotation") * rotateSpeed;

        distance += Input.GetAxis("Zoom");
        height += Input.GetAxis("Zoom") / 2;

        height = Mathf.Clamp(height, height_min, height_max);
        distance = Mathf.Clamp(distance, distance_min, distance_max);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, height, -distance);

        Quaternion rotation = Quaternion.Euler(0, currentX, 0);

        camTransform.position = lookAt.position + rotation * dir;

        camTransform.LookAt(lookAt.position);
    }
}