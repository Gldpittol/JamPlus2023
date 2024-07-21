using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float maxX, maxY, minX, minY;
    public Color bgColor;
    public float ratio = 1.777f;
    private Camera cam;
    public float camOffsetX;

    private float ratioRatio;
    private float currentRatio;
    public float orthoSize;

    private void Awake()
    {
        maxX += camOffsetX;
        minX += camOffsetX;
        cam = GetComponent<Camera>();
        orthoSize = cam.orthographicSize;
        cam.backgroundColor = bgColor;
        Rescale();
        transform.position = new Vector3(camOffsetX, 0, -10);
    }

    private void LateUpdate()
    {
        if (cam.orthographicSize == orthoSize)
        {
            transform.position = new Vector3(camOffsetX, 0, -10);
            return;
        }

        float ratio = (orthoSize - cam.orthographicSize) / 2;
        
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

    public void Rescale()
    { 
        currentRatio = (float)Screen.width / (float)Screen.height; 
        ratioRatio = ratio / currentRatio;
        ratioRatio += 0.09f;
        if (currentRatio < ratio)
        {
            cam.orthographicSize *= ratioRatio;
            maxX *= ratioRatio;
            maxY *= ratioRatio;
            orthoSize = cam.orthographicSize;
            /*maxX *= ratioRatio;
            minX *= ratioRatio;
            maxY *= ratioRatio;
            minY *= ratioRatio;*/
            if (StarsManager.Instance)
            {
                StarsManager.Instance.ChangePillar2();
            }
        }
    }
}
