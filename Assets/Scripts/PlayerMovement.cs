using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    
    [Header("Line Parameters")]
    [SerializeField] private float baseAngle;
    [SerializeField] private float currentAngle;
    [SerializeField] private float incrementStrength;

    [SerializeField] private float minValue, maxValue;
    [SerializeField] private GameObject lineObject;

    [Header("Dash Parameters")]
    [SerializeField] private float dashStrength;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private bool followCoin = true;
    [SerializeField] private float dashBufferTime = 0.2f;

    private Vector2 currentValues; 
    private bool isIncreasing = true;
    private Rigidbody2D rb;
    private Vector2 lineVector;
    private float currentDashTimer;
    private bool isGrounded = true;

    private bool dashBuffered = false;
    private Coroutine dashCoroutine;
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTimers();        
        CalculateAngle();
        CheckInput();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isGrounded = true;
            rb.velocity = Vector2.zero;
            currentAngle = 0;
            foreach (ContactPoint2D contact in other.contacts)
            {
                if (contact.normal.y > 0)
                {
                 //   Debug.Log("Bottom hit");
                    baseAngle = 90;
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.right, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = true;
                        else isIncreasing = false;   
                    }
                }
                else if (contact.normal.y < 0)
                {
//                    Debug.Log("Top hit");
                    baseAngle = 270;
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.right, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = false;
                        else isIncreasing = true;
                    }
                }
                else if (contact.normal.x > 0)
                {
                 //   Debug.Log("Left hit");
                    baseAngle = 0;
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.up, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = false;
                        else isIncreasing = true;
                    }
                }
                else if (contact.normal.x < 0)
                {
                  //  Debug.Log("Right hit");
                    baseAngle = 180;
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.up, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = true;
                        else isIncreasing = false;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KillTrigger"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void UpdateTimers()
    {
        currentDashTimer -= Time.deltaTime;
    }

    public void CalculateAngle()
    {
        if (!IsGrounded()) return;
        
        if (isIncreasing)
        {
            currentAngle += incrementStrength * Time.deltaTime;
        }
        else
        {
            currentAngle -= incrementStrength * Time.deltaTime;
        }

        if (currentAngle >= maxValue)
        {
            currentAngle = maxValue;
            isIncreasing = false;
        }
        if (currentAngle <= minValue)
        {
            currentAngle = minValue;
            isIncreasing = true;
        }
      
        lineObject.transform.eulerAngles = new Vector3(0, 0, currentAngle + baseAngle);
        
        //Debug.DrawRay(transform.position, lineVector.normalized, Color.green, Time.deltaTime);
    }

    public IEnumerator DashBufferCoroutine()
    {
        dashBuffered = true;
        yield return new WaitForSeconds(dashBufferTime);
        dashBuffered = false;
    }

    public void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(IsGrounded())
            {
                AngledDash();
            }
            else
            {
                if(dashCoroutine != null)StopCoroutine(dashCoroutine);
                dashCoroutine = StartCoroutine(DashBufferCoroutine());
            }
        }
        
        if(dashBuffered && IsGrounded())
        {
            AngledDash();
        }
    }

    public void AngledDash()
    {
        if (currentDashTimer > 0) return;
        
        currentDashTimer = dashCooldown;

        rb.AddForce(new Vector2(lineObject.transform.right.x, lineObject.transform.right.y) * dashStrength);
        
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Jump);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public void WaitForGroundedAndSpawnObstacle()
    {
        StartCoroutine(WaitForGroundedAndSpawnObstacleCoroutine());
    }

    public IEnumerator WaitForGroundedAndSpawnObstacleCoroutine()
    {
        while (!IsGrounded())
        {
            yield return null;
        }
        
        GameManager.Instance.SpawnObstacle();
    }
}
