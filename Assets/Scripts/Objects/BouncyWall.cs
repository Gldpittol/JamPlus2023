using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();

            if (invertX)
            {
                playerRb.velocity = new Vector2(-other.relativeVelocity.x, other.relativeVelocity.y);
            }

            if (invertY)
            {
                playerRb.velocity = new Vector2(other.relativeVelocity.x, -other.relativeVelocity.y);
            }
        }
    }
}
