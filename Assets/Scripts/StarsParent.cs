using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarsParent : MonoBehaviour
{
    [SerializeField] private float minSpeed,maxSpeed;
    
    private void Awake()
    {
        foreach (Animator anim in GetComponentsInChildren<Animator>())
        {
            anim.speed = Random.Range(minSpeed, maxSpeed);
            anim.Play("StarAnim");
        }
 
    }
}
