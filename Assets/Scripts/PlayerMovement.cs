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

    private Vector2 currentValues; 
    private bool isIncreasing = true;
    private Rigidbody2D rb;
    private Vector2 lineVector;
    private float currentDashTimer;
    private bool isGrounded = true;
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
            isIncreasing = !isIncreasing;
            isGrounded = true;
            rb.velocity = Vector2.zero;
            currentAngle = 0;
            foreach (ContactPoint2D contact in other.contacts)
            {
                if (contact.normal.y > 0)
                {
                    Debug.Log("Bottom hit");
                    baseAngle = 90;
                }
                else if (contact.normal.y < 0)
                {
                    Debug.Log("Top hit");
                    currentValues.y = -1;
                    baseAngle = 270;
                }
                else if (contact.normal.x > 0)
                {
                    Debug.Log("Left hit");
                    baseAngle = 0;
                }
                else if (contact.normal.x < 0)
                {
                    Debug.Log("Right hit");
                    baseAngle = 180;
                }
            }
            /*lineObject.transform.localScale = new Vector3(lineObject.transform.localScale.x * -1, lineObject.transform.localScale.y, lineObject.transform.localScale.z);
            xValue *= -1;
            yValue *= -1;
            isIncreasing = !isIncreasing;*/
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

    public void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            AngledDash();
        }
    }

    public void AngledDash()
    {
        if (currentDashTimer > 0) return;
        
        currentDashTimer = dashCooldown;

        rb.AddForce(new Vector2(lineObject.transform.right.x, lineObject.transform.right.y) * dashStrength);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
