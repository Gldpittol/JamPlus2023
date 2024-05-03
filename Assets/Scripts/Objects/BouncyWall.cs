using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;

    private ScalePop scalePop;

    private void Awake()
    {
        scalePop = GetComponentInChildren<ScalePop>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 relVel = other.relativeVelocity;

            if (invertX)
            {
                playerRb.velocity = new Vector2(-other.relativeVelocity.x, other.relativeVelocity.y);
            }

            if (invertY)
            {
                playerRb.velocity = new Vector2(other.relativeVelocity.x, -other.relativeVelocity.y);
            }
            
            scalePop.PopOutAnimation();
            //if(AudioManager.Instance) AudioManager.Instance.PlaySound(AudioManager.AudioType.Jump);

            //canCollide = false;
            //StartCoroutine(DisableColliderCoroutine());
            PlayerMovement.Instance.TouchedBouncer(relVel);
            PlayerMovement.Instance.DisableArrow();
            AudioManager.Instance.PlaySound(AudioManager.AudioType.Bounce);
        }
    }

    /*public IEnumerator DisableColliderCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        canCollide = true;
    }*/
}
