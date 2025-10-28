using UnityEngine;
using UnityEngine.Splines;

public class SplineMover : MonoBehaviour
{
    public string MoverName;
    
    [Header("References")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform movingObject;
    
    [Header("Settings")]
    [SerializeField] private float duration = 5f;
    
    private bool hasTriggered = false;
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
    
    void Update()
    {
        // 更新移动
        if (isMoving)
        {
            UpdateMovement();
        }
    }
    
    /// <summary>
    /// 外部调用：开始移动
    /// </summary>
    public void StartMovement()
    {
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
    
    /// <summary>
    /// 外部调用：重置移动状态（允许重新触发）
    /// </summary>
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
}