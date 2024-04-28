using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    public static Coin Instance;
    
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [SerializeField] private int coinsForNewObstacle;
    [SerializeField] private float minRotationSpeedT1, maxRotationSPeedT1;
    [SerializeField] private float minRotationSpeedT2, maxRotationSPeedT2;
    [SerializeField] private float minRotationSpeedT3, maxRotationSPeedT3;
    [SerializeField] private float multiplierSpeedT2;
    [SerializeField] private float multiplierSpeedT3;
    [SerializeField] private Color glowColorT1;
    [SerializeField] private Color glowColorT2;
    [SerializeField] private Color glowColorT3;

    [SerializeField] private GameObject collectVFX;
    [SerializeField] private GameObject permanentVFX;
    [SerializeField] private GameObject previsionObject;
    [SerializeField] private ParticleSystem glowVFX;

    [Header("Constrains")] 
    [SerializeField] private float minDistanceFromLast = 3;
    [SerializeField] private float minObstacleDistance;
    [SerializeField] private List<GameObject> obstacles;

    [Header("Debug")] 
    [SerializeField] private bool enableAKeyDebug = false;

    private int coinsCollected;
    private ScalePop scalePop;
    private float rotationSpeed;
    private int extraFactor = 1;
    private bool collected = false;
    private Vector2 nextPos;

    private void Awake()
    {
        Instance = this;
        GetNewPos(0);
        previsionObject.transform.position = nextPos;
        previsionObject.transform.parent = null;
        //SlowRotate();
    }

    private void Start()
    {
        RandomizeRotation();
        scalePop = GetComponent<ScalePop>();
        scalePop.PopOutAnimation();
    }

    private void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, rotationSpeed) * Time.deltaTime * extraFactor;
        previsionObject.transform.eulerAngles += new Vector3(0, 0, rotationSpeed) * Time.deltaTime * extraFactor;
        
        if(enableAKeyDebug) if(Input.GetKeyDown(KeyCode.A)) Collect();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (collected) return;
        if (GameManager.Instance.gameState == GameManager.GameState.GameEnded) return;
        
        if (other.CompareTag("Player"))
        {
           Collect();
        }
    }

    public void Collect()
    {
        collected = true;
        coinsCollected++;
        GameObject tempVFX = Instantiate(collectVFX, transform.position, Quaternion.identity);
        tempVFX.transform.eulerAngles = collectVFX.transform.eulerAngles;
        Destroy(tempVFX, 1f);
        GameManager.Instance.UpdateScore();
        ComboBar.Instance.ResetDelay();
        ComboBar.Instance.Increment();
        ComboBar.Instance.DoComboText();
        transform.position = nextPos;
        GetNewPos(0);
        previsionObject.transform.position = nextPos;
        scalePop.PopOutAnimation();
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Collect);
        PlayerMovement.Instance.IncreaseIncrement();
        RandomizeRotation();

        if (coinsCollected % coinsForNewObstacle == 0)
        {
            PlayerMovement.Instance.WaitForGroundedAndSpawnObstacle();
        }

        collected = false;
    }

    private void GetNewPos(int iter)
    {
        Vector2 newPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
        if(iter >= 20) transform.position = newPos;

        if (Vector2.Distance(newPos, transform.position) < minDistanceFromLast)
        {
            GetNewPos(iter + 1);
            return;
        }
        
        foreach (GameObject g in obstacles)
        {
            if (Vector2.Distance(newPos, g.transform.position) < minObstacleDistance)
            {
                GetNewPos(iter + 1);
                return;
            }
        }

        nextPos = newPos;
    }

    public void RandomizeRotation()
    {
        int id = ComboBar.Instance.GetListID(ComboBar.Instance.GetComboMultiplier());
        if (id == -1) id = 2;
        if (id == 0)
        {
            rotationSpeed = Random.Range(minRotationSpeedT3, maxRotationSPeedT3);
            foreach (ParticleSystem ps in permanentVFX.GetComponentsInChildren<ParticleSystem>())
            {
                var mainModule = ps.main;
                mainModule.simulationSpeed = multiplierSpeedT3;
            }

            var glowVFXMain = glowVFX.main;
            glowVFXMain.startColor = glowColorT3;
        }
        else if (id == 1)
        {
            rotationSpeed = Random.Range(minRotationSpeedT2, maxRotationSPeedT2);
            foreach (ParticleSystem ps in permanentVFX.GetComponentsInChildren<ParticleSystem>())
            {
                var mainModule = ps.main;
                mainModule.simulationSpeed = multiplierSpeedT2;
            }
            var glowVFXMain = glowVFX.main;
            glowVFXMain.startColor = glowColorT2;
        }
        else if (id == 2)
        {
            rotationSpeed = Random.Range(minRotationSpeedT1, maxRotationSPeedT1);
            var glowVFXMain = glowVFX.main;
            glowVFXMain.startColor = glowColorT1;
        }
        
        if (Random.value < 0.5f) extraFactor *= -1;
    }

    /*public void SlowRotate()
    {
        transform.DORotate(new Vector3(0,0,180), slowRotateDuration, RotateMode.FastBeyond360).OnComplete(()=>SlowRotate());
    }*/
}
