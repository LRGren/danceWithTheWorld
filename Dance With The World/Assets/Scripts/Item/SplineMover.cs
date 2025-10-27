using System;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class SplineMover : MonoBehaviour
{
    [Header("Spline Settings")]
    public string splineName;
    public SplineContainer spline;
    public float duration = 3f;
    
    public bool doneMovement = false;
    
    private Coroutine moveCoroutine;

    private void Update()
    {
        
    }

    /// <summary>
    /// 触发移动 - 只需调用一次
    /// </summary>
    public void Move()
    {
        if(doneMovement)
            return;
        
        if (spline == null)
        {
            Debug.LogError("Spline未分配！");
            return;
        }
        
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        doneMovement = true;
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    /// <summary>
    /// 触发移动并指定Spline
    /// </summary>
    public void Move(SplineContainer targetSpline, float moveDuration = -1)
    {
        if (targetSpline != null) 
            spline = targetSpline;
        
        if (moveDuration > 0) 
            duration = moveDuration;

        Move();
    }

    private IEnumerator MoveRoutine()
    {
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            // 获取Spline上的位置
            Vector3 position = spline.EvaluatePosition(t);
            position = spline.transform.TransformPoint(position);
            
            // 移动物体
            transform.position = position;
            
            yield return null;
        }
        
        // 确保到达终点
        Vector3 endPosition = spline.EvaluatePosition(1f);
        endPosition = spline.transform.TransformPoint(endPosition);
        transform.position = endPosition;
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public void Stop()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
}