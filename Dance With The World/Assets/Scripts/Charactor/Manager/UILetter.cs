using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILetter : MonoBehaviour
{
    [Header("UI组件")]
    public Image letterImage;
    public CanvasGroup canvasGroup;

    private RectTransform rectTransform;
    private float fadeTimer;
    private float originalFadeDuration;
    private float currentFadeDuration;
    
    // 动画参数
    private float scaleInDuration;
    private float scaleOutDuration;
    private AnimationCurve scaleCurve;
    private float scaleTimer = 0f;
    private bool isScalingIn = true;
    private bool isScalingOut = false;

    // 音效回调
    public System.Action OnFadeOut;

    public bool IsDestroyed { get; private set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        if (letterImage == null)
            letterImage = GetComponentInChildren<Image>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Initialize(Sprite sprite, float fadeTime, float scaleInTime, float scaleOutTime, 
                          AnimationCurve curve, Vector2 position)
    {
        if (letterImage != null)
            letterImage.sprite = sprite;

        originalFadeDuration = fadeTime;
        currentFadeDuration = fadeTime;
        fadeTimer = fadeTime;
        scaleInDuration = scaleInTime;
        scaleOutDuration = scaleOutTime;
        scaleCurve = curve;

        // 设置初始位置
        rectTransform.anchoredPosition = position;

        // 初始状态
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        IsDestroyed = false;
        isScalingIn = true;
        isScalingOut = false;
        scaleTimer = 0f;

        // 初始缩放为0
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (IsDestroyed) return;

        // 缩放动画
        UpdateScaleAnimation();

        // 渐隐效果
        UpdateFade();
    }

    void UpdateScaleAnimation()
    {
        if (isScalingIn)
        {
            scaleTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(scaleTimer / scaleInDuration);
            float scaleValue = scaleCurve.Evaluate(progress);
            
            transform.localScale = Vector3.one * scaleValue;

            if (progress >= 1f)
            {
                isScalingIn = false;
                scaleTimer = 0f;
            }
        }
        else if (isScalingOut)
        {
            scaleTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(scaleTimer / scaleOutDuration);
            float scaleValue = scaleCurve.Evaluate(1f - progress);
            
            transform.localScale = Vector3.one * scaleValue;
        }
        // 在消失前一段时间开始缩放消失
        else if (fadeTimer < scaleOutDuration)
        {
            isScalingOut = true;
            scaleTimer = 0f;
        }
    }

    void UpdateFade()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = fadeTimer / currentFadeDuration;
            }
        }
        else
        {
            DestroyLetter();
        }
    }

    public void SetPosition(Vector2 position)
    {
        // 直接设置位置，没有移动动画
        rectTransform.anchoredPosition = position;
    }

    public void AccelerateFade(float acceleration)
    {
        currentFadeDuration = originalFadeDuration / acceleration;
        fadeTimer = Mathf.Min(fadeTimer, currentFadeDuration);
    }

    void DestroyLetter()
    {
        if (!IsDestroyed)
        {
            // 触发消失音效回调
            OnFadeOut?.Invoke();
            
            IsDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void DestroyImmediate()
    {
        if (!IsDestroyed)
        {
            IsDestroyed = true;
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        IsDestroyed = true;
    }
}