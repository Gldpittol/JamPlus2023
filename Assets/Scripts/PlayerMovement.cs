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
    [SerializeField] private float yIncrement;
    [SerializeField] private float minY, maxY;
    [SerializeField] private GameObject lineObject;

    [Header("Dash Parameters")]
    [SerializeField] private float dashStrength;
    [SerializeField] private float dashCooldown = 0.3f;

    private float xValue = 1, yValue;
    private bool isIncreasing = true;
    private Rigidbody2D rb;
    private Vector2 lineVector;
    private float currentDashTimer;
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
            //transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            xValue *= -1;
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
        if (isIncreasing)
        {
            yValue += yIncrement * Time.deltaTime;
        }
        else
        {
            yValue -= yIncrement * Time.deltaTime;
        }

        if (yValue >= maxY)
        {
            yValue = maxY;
            isIncreasing = false;
        }
        if (yValue <= minY)
        {
            yValue = minY;
            isIncreasing = true;
        }

        lineVector = new Vector2(xValue, yValue).normalized;

        lineObject.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(transform.right, lineVector));
        
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
        rb.AddForce(lineVector * dashStrength);
    }

    public bool IsGrounded()
    {
        return rb.velocity.x < 0.01f;
    }
}
