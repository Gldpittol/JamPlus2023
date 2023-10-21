using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private float increasePerCoin = 1;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float radius = 0.45f;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private GameObject raycastSource;
    [SerializeField] private SpriteRenderer playerRenderer;

    [Header("Dash Parameters")]
    [SerializeField] private float dashStrength;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private bool followCoin = true;
    [SerializeField] private float dashBufferTime = 0.2f;
  
    [Header("Player Visual Offset")]
    [SerializeField] private Vector2 offsetBottom;
    [SerializeField] private Vector2 offsetTop;
    [SerializeField] private Vector2 offsetRight;
    [SerializeField] private Vector2 offsetLeft;

    
    private Animator animator;
    private Vector2 currentValues; 
    private bool isIncreasing = true;
    private Rigidbody2D rb;
    private Vector2 lineVector;
    private float currentDashTimer;
    private bool isGrounded = true;
    private bool dashBuffered = false;
    private Coroutine dashCoroutine;
    private SpriteRenderer lineObjectRenderer;
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        lineObjectRenderer = lineObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance.LevelEnded) return;
        UpdateTimers();        
        CalculateAngle();
        CheckInput();
    }

    private void FixedUpdate()
    {
        DrawRayCasts();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if(!isGrounded)animator.Play("IdleAnim");

            isGrounded = true;

            rb.velocity = Vector2.zero;
            currentAngle = 0;
            foreach (ContactPoint2D contact in other.contacts)
            {
                playerRenderer.flipX = false;
                if (contact.normal.y > 0)
                {
                    //   Debug.Log("Bottom hit");
                    if (lineObject.transform.right.x < 0)
                    {
                        playerRenderer.flipX = true;
                    }
                    baseAngle = 90;
                    playerRenderer.gameObject.transform.eulerAngles = new Vector3(0, 0, baseAngle-90);
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.right, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = true;
                        else isIncreasing = false;   
                    }

                    playerRenderer.gameObject.transform.localPosition = offsetBottom;
                }
                else if (contact.normal.y < 0)
                {
//                    Debug.Log("Top hit");
                    if (lineObject.transform.right.x > 0)
                    {
                        playerRenderer.flipX = true;
                    }
                    baseAngle = 270;
                    playerRenderer.gameObject.transform.eulerAngles = new Vector3(0, 0, baseAngle-90);
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.right, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = false;
                        else isIncreasing = true;
                    }
                    playerRenderer.gameObject.transform.localPosition = offsetTop;

                }
                else if (contact.normal.x > 0)
                {
                    //   Debug.Log("Left hit");
                    if (lineObject.transform.right.y > 0)
                    {
                        playerRenderer.flipX = true;
                    }
                   
                    baseAngle = 0;
                    playerRenderer.gameObject.transform.eulerAngles = new Vector3(0, 0, baseAngle-90);
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.up, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = false;
                        else isIncreasing = true;
                    }
                    playerRenderer.gameObject.transform.localPosition = offsetLeft;

                }
                else if (contact.normal.x < 0)
                {
                    //  Debug.Log("Right hit");
                    if (lineObject.transform.right.y < 0)
                    {
                        playerRenderer.flipX = true;
                    }
                    baseAngle = 180;
                    playerRenderer.gameObject.transform.eulerAngles = new Vector3(0, 0, baseAngle-90);
                    if (followCoin)
                    {
                        float angleToCoin = Vector2.Angle(Vector2.up, Coin.Instance.transform.position - transform.position);
                        if (angleToCoin > 90) isIncreasing = true;
                        else isIncreasing = false;
                    }
                    playerRenderer.gameObject.transform.localPosition = offsetRight;

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
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            GameManager.Instance.FinishLevel(true);
            animator.Play("Dead");
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
        
        animator.Play("JumpPrep");
        currentDashTimer = dashCooldown;

        rb.AddForce(new Vector2(lineObject.transform.right.x, lineObject.transform.right.y) * dashStrength);
        
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Jump);
        SetCharacterOrientation();
    }

    public void SetCharacterOrientation()
    {
        playerRenderer.gameObject.transform.eulerAngles = Vector3.zero;

        if (lineObject.transform.right.x < 0)
        {
            playerRenderer.flipX = true;
        }
        else
        {
            playerRenderer.flipX = false;
        }
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

    public void DrawRayCasts()
    {
        RaycastHit2D hit = Physics2D.CircleCast(raycastSource.transform.position, 
            radius,lineObject.transform.right, 
            10000,raycastMask);
        if (hit)
        {
           // print(hit.transform.gameObject.name);
            if (hit.transform.CompareTag("Coin"))
            {
                lineObjectRenderer.color = Color.green;
            }
            else
            {
                lineObjectRenderer.color = Color.white;
            }
        }
    }

    public void IncreaseIncrement()
    {
        incrementStrength += increasePerCoin;
        if (incrementStrength > maxSpeed)
        {
            incrementStrength = maxSpeed;
        }
    }

    public void Die(float playerDeathAngle, float delayBeforeGoingToNextLevel)
    {
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.None;
        lineObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        transform.DOLocalRotate(new Vector3(0,0,playerDeathAngle), delayBeforeGoingToNextLevel, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }
}
