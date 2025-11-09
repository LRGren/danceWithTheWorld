using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILetterDisplayManager : MonoBehaviour
{
    public static UILetterDisplayManager instance;
    
    [Header("开关设置")]
    public KeyCode toggleKey = KeyCode.P;
    public Text statusText;
    private bool isActive = false;

    [Header("显示位置设置")] 
    public GameObject s;
    public DisplayPosition displayPosition = DisplayPosition.Center;
    public RectTransform displayRoot;
    public Vector2 customPosition = Vector2.zero;
    public float letterSpacing = 120f;
    public float fadeDuration = 2f;

    [Header("动画设置")]
    public float scaleInDuration = 0.3f;
    public float scaleOutDuration = 0.2f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("字母预制体")]
    public GameObject uiLetterPrefab;
    public Sprite wSprite, aSprite, sSprite, dSprite;
    public Sprite uSprite, iSprite, jSprite, kSprite, lSprite;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip toggleSound;          // 系统开关音效
    public AudioClip letterCreateSound;    // 字母创建音效
    public AudioClip letterFadeOutSound;   // 字母消失音效
    [Range(0f, 1f)]
    public float volume = 0.7f;

    // 显示位置枚举
    public enum DisplayPosition
    {
        Center,
        CenterLeft,
        CenterRight,
        TopCenter,
        TopLeft,
        TopRight,
        BottomCenter,
        BottomLeft,
        BottomRight,
        Custom
    }

    private Dictionary<char, Sprite> letterSpriteMap;
    private List<UILetter> activeLetters = new List<UILetter>();
    private Vector2 basePosition = Vector2.zero;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        
        InitializeSpriteMap();
        InitializeAudio();
        UpdateStatusDisplay();
        
        if (displayRoot != null)
        {
            canvasRect = displayRoot.parent as RectTransform;
            if (canvasRect == null)
                canvasRect = displayRoot;
        }
        
        UpdateDisplayPosition();
    }

    void InitializeAudio()
    {
        // 如果没有AudioSource，自动添加一个
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if(GameManager.instance.isUI)
            return;
        
        if (Input.GetKeyDown(toggleKey))
        {
            s.gameObject.SetActive(!s.gameObject.activeSelf);
            ToggleSystem();
        }

        if (isActive)
        {
            CheckLetterInput();
        }

        UpdateActiveLetters();
    }

    void InitializeSpriteMap()
    {
        letterSpriteMap = new Dictionary<char, Sprite>
        {
            {'w', wSprite}, {'a', aSprite}, {'s', sSprite}, {'d', dSprite},
            {'u', uSprite}, {'i', iSprite}, {'j', jSprite}, {'k', kSprite}, {'l', lSprite}
        };
    }

    void ToggleSystem()
    {
        isActive = !isActive;
        
        // 播放开关音效
        if (toggleSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(toggleSound, volume);
        }
        
        UpdateStatusDisplay();
        
        if (!isActive)
        {
            ClearAllLetters();
        }
    }

    void UpdateStatusDisplay()
    {
        if (statusText != null)
        {
            statusText.text = $"字母显示: {(isActive ? "开启" : "关闭")}\n按 {toggleKey} 切换";
            statusText.color = isActive ? Color.green : Color.red;
        }
    }

    void CheckLetterInput()
    {
        string validLetters = "wasduijkl";
        foreach (char c in validLetters)
        {
            if (Input.GetKeyDown(c.ToString()))
            {
                CreateUILetter(c);
                break;
            }
        }
    }

    void CreateUILetter(char letter)
    {
        if (!letterSpriteMap.ContainsKey(letter) || letterSpriteMap[letter] == null)
        {
            Debug.LogWarning($"字母 {letter} 的图片未设置!");
            return;
        }

        // 播放字母创建音效
        if (letterCreateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(letterCreateSound, volume);
        }

        // 创建UI字母对象
        GameObject letterObj = Instantiate(uiLetterPrefab, displayRoot);
        UILetter uiLetter = letterObj.GetComponent<UILetter>();
        
        if (uiLetter == null)
        {
            uiLetter = letterObj.AddComponent<UILetter>();
        }

        // 设置字母消失时的音效回调
        uiLetter.OnFadeOut = () => 
        {
            if (letterFadeOutSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(letterFadeOutSound, volume * 0.5f); // 消失音效音量减半
            }
        };

        // 新字母总是出现在最左边（位置0）
        Vector2 spawnPosition = basePosition;
        
        // 初始化字母（直接在目标位置出现）
        uiLetter.Initialize(
            letterSpriteMap[letter], 
            fadeDuration,
            scaleInDuration,
            scaleOutDuration,
            scaleCurve,
            spawnPosition
        );

        // 将新字母插入到列表开头（最左边）
        activeLetters.Insert(0, uiLetter);

        // 更新所有字母位置：新字母在左边，旧字母往右挤
        UpdateAllLetterPositions();
    }

    Vector2 CalculateLetterPosition(int index)
    {
        // 索引0在最左边，索引越大越往右
        float horizontalOffset = index * letterSpacing;
        return basePosition + new Vector2(horizontalOffset, 0);
    }

    void UpdateAllLetterPositions()
    {
        // 重新计算所有字母的位置
        for (int i = 0; i < activeLetters.Count; i++)
        {
            if (activeLetters[i] != null && !activeLetters[i].IsDestroyed)
            {
                Vector2 newPosition = CalculateLetterPosition(i);
                activeLetters[i].SetPosition(newPosition);

                // 加速旧字母的消失（除了最新的字母）
                if (i > 0) // 从第二个字母开始加速（索引0是最新的）
                {
                    float acceleration = 1f + (i * 0.8f); // 越往右的字母消失越快
                    activeLetters[i].AccelerateFade(acceleration);
                }
            }
        }
    }

    void UpdateDisplayPosition()
    {
        if (canvasRect == null) return;

        Vector2 canvasSize = canvasRect.rect.size;
        
        switch (displayPosition)
        {
            case DisplayPosition.Center:
                basePosition = Vector2.zero;
                break;
            case DisplayPosition.CenterLeft:
                basePosition = new Vector2(-canvasSize.x * 0.3f, 0);
                break;
            case DisplayPosition.CenterRight:
                basePosition = new Vector2(canvasSize.x * 0.3f, 0);
                break;
            case DisplayPosition.TopCenter:
                basePosition = new Vector2(0, canvasSize.y * 0.3f);
                break;
            case DisplayPosition.TopLeft:
                basePosition = new Vector2(-canvasSize.x * 0.3f, canvasSize.y * 0.3f);
                break;
            case DisplayPosition.TopRight:
                basePosition = new Vector2(canvasSize.x * 0.3f, canvasSize.y * 0.3f);
                break;
            case DisplayPosition.BottomCenter:
                basePosition = new Vector2(0, -canvasSize.y * 0.3f);
                break;
            case DisplayPosition.BottomLeft:
                basePosition = new Vector2(-canvasSize.x * 0.3f, -canvasSize.y * 0.3f);
                break;
            case DisplayPosition.BottomRight:
                basePosition = new Vector2(canvasSize.x * 0.3f, -canvasSize.y * 0.3f);
                break;
            case DisplayPosition.Custom:
                basePosition = customPosition;
                break;
        }
    }

    void UpdateActiveLetters()
    {
        for (int i = activeLetters.Count - 1; i >= 0; i--)
        {
            if (activeLetters[i] == null || activeLetters[i].IsDestroyed)
            {
                activeLetters.RemoveAt(i);
            }
        }
    }

    void ClearAllLetters()
    {
        foreach (UILetter letter in activeLetters)
        {
            if (letter != null)
            {
                letter.DestroyImmediate();
            }
        }
        activeLetters.Clear();
    }

    // 在Inspector中修改位置时更新
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateDisplayPosition();
            UpdateAllLetterPositions();
        }
    }

    public void SetSystemActive(bool active)
    {
        isActive = active;
        
        // 播放开关音效
        if (toggleSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(toggleSound, volume);
        }
        
        UpdateStatusDisplay();
        if (!isActive) ClearAllLetters();
    }

    // 公开方法：动态改变显示位置
    public void ChangeDisplayPosition(DisplayPosition newPosition)
    {
        displayPosition = newPosition;
        UpdateDisplayPosition();
        UpdateAllLetterPositions();
    }

    public void ChangeDisplayPosition(Vector2 customPos)
    {
        displayPosition = DisplayPosition.Custom;
        customPosition = customPos;
        UpdateDisplayPosition();
        UpdateAllLetterPositions();
    }

    // 公开方法：播放自定义音效
    public void PlayCustomSound(AudioClip clip, float customVolume = -1f)
    {
        if (clip != null && audioSource != null)
        {
            float playVolume = customVolume >= 0 ? customVolume : volume;
            audioSource.PlayOneShot(clip, playVolume);
        }
    }
}