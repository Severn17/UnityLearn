﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{
    protected bool isMoveing = false;
    private Vector3 targetPosition;
    public float speed = 1.2f;
    private Animator animator;
    public string desc = "";

    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoveing = true;
        animator.SetBool("isMoving",true);
    }

    public void MoveUpdate()
    {
        if (isMoveing==false)
        {
            return;
        }

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(pos,targetPosition)< 0.05f)
        {
            isMoveing = false;
            animator.SetBool("isMoving",false);
        }
    }

    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected void Update()
    {
        MoveUpdate();
    }
}
