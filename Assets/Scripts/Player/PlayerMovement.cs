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
    [SerializeField] private SpriteRenderer lineColor;
    [SerializeField] private GameObject lineMask;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private float outlineTweenDuration = 0.1f;
    [SerializeField] private Color colorAimed;
    [SerializeField] private Color colorNotAimed;
    [SerializeField] private ParticleSystem dustVFX;

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

    [Header("Death Juiceness")]
    [SerializeField] private float timeOnSlowMotion;
    [SerializeField] private float slowMotionTimeScale;
    [SerializeField] private float maxCameraZoomInDuration;
    [SerializeField] private float maxCameraZoomIn;
    [SerializeField] private float cameraZoomOutDuration;

    [Header("Arrow New")] 
    public GameObject arrowMask;
    public GameObject centerGlow;
    public float maskMoveXAmount ;

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
    private bool isFirst = true;
    private bool isDead = false;
    private bool canSwapAnimation = true;

    private float initialDelay = 0.1f;
    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        lineObjectRenderer = lineObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        initialDelay -= Time.deltaTime;
        if (initialDelay > 0) return;
        if(isDead) dustVFX.gameObject.SetActive(false);

        if (GameManager.Instance.LevelEnded) return;
        UpdateTimers();        
        CalculateAngle();
        CheckInput();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.LevelEnded) return;

        DrawRayCasts();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (!isGrounded)
            {
                if(canSwapAnimation) animator.Play("IdleAnim");
                isGrounded = true;
                rb.velocity = Vector2.zero;
            }
            
            currentAngle = 0;
            
            if (isFirst)
            {
                isFirst = !isFirst;
            }
            else
            {
                dustVFX.gameObject.SetActive(true);
                dustVFX.Play();
                playerRenderer.gameObject.GetComponent<ScalePop>().PopOutAnimation();
                GameManager.Instance.DoScreenShake();
            }

            ContactPoint2D contact = other.contacts[0];
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

    public void CheckIfSwapToIdle()
    {
        if (isGrounded)
        {
            animator.Play("IdleAnim");
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
            if (canSwapAnimation)
            {
                animator.Play("Dead");
                canSwapAnimation = false;
            }
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
        if(Input.GetKeyDown(KeyCode.Space)  || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            InputPerformed();
        }
        
        if(dashBuffered && IsGrounded())
        {
            AngledDash();
        }
    }

    public void InputPerformed()
    {
        if (initialDelay > 0) return;
        if (GameManager.Instance.LevelEnded) return;

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

    public void AngledDash()
    {
        StartCoroutine(AngledDashCoroutine());
    }

    public IEnumerator AngledDashCoroutine()
    {
        if (currentDashTimer > 0) yield break;
        currentDashTimer = dashCooldown;
        
        yield return new WaitForEndOfFrame();

        rb.AddForce(new Vector2(lineObject.transform.right.x, lineObject.transform.right.y) * dashStrength);

//        print("Aqui");
        if(canSwapAnimation)animator.Play("JumpAnim");
        currentDashTimer = dashCooldown;
        
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Jump);
        SetCharacterOrientation();
        dustVFX.Stop();
        dustVFX.gameObject.SetActive(false);
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
        //return;
        RaycastHit2D hit = Physics2D.CircleCast(raycastSource.transform.position, 
            radius,lineObject.transform.right, 
            10000,raycastMask);
        if (hit)
        {
           // print(hit.transform.gameObject.name);
            if (hit.transform.CompareTag("Coin"))
            {
                lineObjectRenderer.DOColor(colorAimed, outlineTweenDuration);
            }
            else
            {
                lineObjectRenderer.DOColor(colorNotAimed, outlineTweenDuration);
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
        isDead = true;
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Death);
        AudioManager.Instance.PlaySound(AudioManager.AudioType.CatDeath);

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.None;
        DisableArrow();
        GetComponent<Collider2D>().enabled = false;
        transform.DOLocalRotate(new Vector3(0,0,playerDeathAngle), delayBeforeGoingToNextLevel, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        DeathJuiceness();
        dustVFX.gameObject.SetActive(false);
    }

    public void DeathJuiceness()
    {
        StartCoroutine(DeathJuicenessCoroutine());
    }

    public IEnumerator DeathJuicenessCoroutine()
    {
        yield return null;
        Time.timeScale = slowMotionTimeScale;
        Camera.main.DOOrthoSize(maxCameraZoomIn, maxCameraZoomInDuration).OnComplete(()=>
            Camera.main.DOOrthoSize(5, cameraZoomOutDuration));
        Camera.main.transform.DOMove(new Vector3(transform.position.x, transform.position.y, -10), maxCameraZoomInDuration).OnComplete(()=>
            Camera.main.transform.DOMove(new Vector3(0,0, -10), cameraZoomOutDuration));
        yield return new WaitForSecondsRealtime(timeOnSlowMotion);
        Time.timeScale = 1;
    }

   

    public void SetArrowCountdown(Color startColor, Color endColor, float duration)
    {
        //lineColor.color = startColor;
        //lineColor.DOColor(endColor, duration* 2);
        arrowMask.transform.DOLocalMoveX(0, duration).SetEase(Ease.Linear);
        //lineMask.transform.DOScaleX(0, duration * 2);
    }

    public void SetArrowFlash()
    {
        centerGlow.SetActive(true);
        /*lineColor.DOFade(0, flashDuration)
            .OnComplete(() => lineColor.DOFade(1, flashDuration).OnComplete(() => SetArrowFlash()));*/
    }

    public void DisableArrow()
    {
        foreach (SpriteRenderer sr in lineObject.GetComponentsInChildren<SpriteRenderer>(true))
        {
            sr.enabled = false;
        }

        centerGlow.SetActive(false);
    }
}
