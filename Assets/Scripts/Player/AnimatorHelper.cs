using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void CheckIfSwapToIdle()
    {
        GetComponentInParent<PlayerMovement>().CheckIfSwapToIdle();
    }
}
