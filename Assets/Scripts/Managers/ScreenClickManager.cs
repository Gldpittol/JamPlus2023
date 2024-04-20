using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenClickManager : MonoBehaviour
{
    public static ScreenClickManager Instance;

    [SerializeField] private GameObject clickVFX;
    [SerializeField] private bool destroyWhenSpawningNew = true;

    private Vector3 screenClickPos;
    private Touch screenTouch;
    private GameObject tempVFX;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Gameplay && !HUDManager.Instance.IsPaused)
        {
            return;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            GetClickPosition();
            InstantiateVFX();
        }

        if (Input.touchCount > 0)
        {
            GetTouchPosition();
        }
    }

    private void GetClickPosition()
    {
        screenClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenClickPos.z = 10.0f;
    }

    public void GetTouchPosition()
    {
        screenTouch = Input.GetTouch(0);
        screenClickPos = Camera.main.ScreenToWorldPoint(screenTouch.position);
        screenClickPos.z = 10.0f;
        if (screenTouch.phase == TouchPhase.Ended)
        {
            InstantiateVFX();
        }
    }

    private void InstantiateVFX()
    {
        if (destroyWhenSpawningNew) Destroy(tempVFX);
        if (clickVFX)
        {
            tempVFX = Instantiate(clickVFX, screenClickPos, Quaternion.identity);
            tempVFX.transform.parent = transform;
            StartCoroutine(DestroyAfterXSeconds(tempVFX));
        }
    }

    public IEnumerator DestroyAfterXSeconds(GameObject vfx)
    {
        yield return new WaitForSecondsRealtime(2f);
        Destroy(vfx);
    }
}
