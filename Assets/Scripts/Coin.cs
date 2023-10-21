using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private int coinsCollected;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            coinsCollected++;
            transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
            GameManager.Instance.UpdateScore();
            ComboBar.Instance.ResetDelay();
            ComboBar.Instance.Increment();
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Collect);
            PlayerMovement.Instance.IncreaseIncrement();

            if (coinsCollected % coinsForNewObstacle == 0)
            {
                PlayerMovement.Instance.WaitForGroundedAndSpawnObstacle();
            }
        }
    }
}
