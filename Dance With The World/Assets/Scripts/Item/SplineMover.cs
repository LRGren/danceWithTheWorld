using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : SkillItem
{
    [Header("References")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform movingObject;
    [SerializeField] protected bool canMove = false;
    
    [Header("Settings")]
    [SerializeField] private float duration = 5f;
    
    private bool isMoving = false;
    private float currentTime = 0f;

    void Start()
    {
        // 自动查找引用
        if (splineContainer == null)
            splineContainer = GetComponentInChildren<SplineContainer>();
            
        if (movingObject == null)
            movingObject = transform;
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 更新移动
        if (isMoving)
        {
            UpdateMovement();
        }
    }

    protected override void SkillOn()
    {
        base.SkillOn();
        
        print("go");
        
        StartMovement();
    }

    public void StartMovement()
    {
        if(!canMove)
            return;
        
        if (splineContainer == null)
        {
            Debug.LogWarning("SplineContainer未设置！");
            return;
        }
        
        if (hasTriggered || isMoving) return;
        
        hasTriggered = true;
        isMoving = true;
        currentTime = 0f;
    }
    
    public void ResetMovement()
    {
        hasTriggered = false;
        isMoving = false;
        currentTime = 0f;
    }
    
    private void UpdateMovement()
    {
        currentTime += Time.deltaTime;
        float normalizedTime = currentTime / duration;
        
        if (normalizedTime >= 1f)
        {
            // 移动完成
            isMoving = false;
            normalizedTime = 1f;
        }
        
        // 沿Spline移动
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);
        Vector3 tangent = splineContainer.EvaluateTangent(normalizedTime);
        
        movingObject.position = position;
        
        // 设置朝向
        if (tangent != Vector3.zero)
        {
            movingObject.rotation = Quaternion.LookRotation(tangent.normalized);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = false;
        }
    }
}