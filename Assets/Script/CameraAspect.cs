using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraAspect : MonoBehaviour
{
    Camera mainCamera;
    Vector2 aspect = new Vector2(21, 9);
    void Start()
    {
        mainCamera = Camera.main;
        var h = 16.0f / 9.0f * aspect.y / aspect.x;
        var rect = new Rect(0, (1 - h) * 0.5f, 1, h);
        mainCamera.rect = rect;
    }
}
