using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float maxX, maxY, minX, minY;
    public Color bgColor;

    private Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = bgColor;
    }

    private void LateUpdate()
    {
        if (cam.orthographicSize == 5)
        {
            transform.position = new Vector3(0, 0, -10);
            return;
        }

        float ratio = (5 - cam.orthographicSize) / 2;
        
        if (transform.position.x > maxX * ratio)
        {
            transform.position = new Vector3(maxX * ratio, transform.position.y, -10);
        }
        if (transform.position.x < minX  * ratio)
        {
            transform.position = new Vector3(minX  * ratio, transform.position.y, -10);
        }
        if (transform.position.y > maxY  * ratio)
        {
            transform.position = new Vector3(transform.position.x, maxY  * ratio, -10);
        }
        if (transform.position.y < minY  * ratio)
        {
            transform.position = new Vector3(transform.position.x, minY  * ratio, -10);
        }
    }
}
