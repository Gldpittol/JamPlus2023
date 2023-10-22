using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    public static Coin Instance;
    
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [SerializeField] private int coinsForNewObstacle;
    [SerializeField] private float slowRotateDuration;

    private int coinsCollected;
    
    private void Awake()
    {
        Instance = this;
        SlowRotate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            coinsCollected++;
            GameManager.Instance.UpdateScore();
            ComboBar.Instance.ResetDelay();
            ComboBar.Instance.Increment();
            ComboBar.Instance.DoComboText();
            transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Collect);
            PlayerMovement.Instance.IncreaseIncrement();

            if (coinsCollected % coinsForNewObstacle == 0)
            {
                PlayerMovement.Instance.WaitForGroundedAndSpawnObstacle();
            }
        }
    }

    public void SlowRotate()
    {
        transform.DORotate(new Vector3(0,0,1000), slowRotateDuration, RotateMode.FastBeyond360).OnComplete(()=>SlowRotate());
    }
}
