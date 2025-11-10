using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;

    [Header("Global Information")] [SerializeField]
    private float TimeLine = 0;

    public bool appearFlashTrigger = false;
    public bool appearConditionedTrigger = false;

    [Header("UI References")]
    public GameObject mainUI;
    public GameObject hisObj;
    public Text History;
    public GameObject flashTextObject;
    public Text flashText;
    public float flashTime = 0;
    public float setFlashTime = 3f;
    public GameObject conditionedTextObject;
    public Text conditionedText;
    public float conditionedTime = 0;
    public KeyCode conditionedKey = KeyCode.None;

    [Header("Settings")] 
    public string flashTextInfo;
    public string conditionedTextInfo;

    public List<TextTrigger>  textTriggers = new List<TextTrigger>();
    public TextTrigger current;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (textTriggers.Count > 0)
        {
            current = textTriggers[0];
        }
        else
        {
            current = null;
        }

        if (current != null)
        {
            if(textTriggers[0]==null)
                textTriggers.RemoveAt(0);
            
            if (!current.haveDone && !conditionedTextObject.activeSelf && !flashTextObject.activeSelf) 
            {
                if (current.type == 0)
                {
                    appearFlashTrigger = true;
                    
                    flashTextInfo = current.Info;
                    
                    textTriggers.RemoveAt(0);
                    current.haveDone = true;
                }

                if (current.type == 1)
                {
                    appearConditionedTrigger = true;
                    
                    conditionedTextInfo = current.Info;
                    conditionedKey = current.key;
                    
                    textTriggers.RemoveAt(0);
                    current.haveDone = true;
                }
                
            }
        }
        
        if (appearFlashTrigger)
        {
            //flash
            AppearFlashTrigger();
        }

        if (appearConditionedTrigger)
        {
            //conditioned
            AppearConditionedTrigger();
        }

        if (conditionedTextObject.activeSelf)
        {
            if (Input.GetKeyDown(conditionedKey))
            {
                conditionedKey = KeyCode.None;
                conditionedTextObject.SetActive(false);
            }
        }
    }

    private void AppearFlashTrigger()
    {
        if(conditionedTextObject.gameObject.activeSelf)
            return;
        
        flashTextObject.SetActive(true);


        History.text += flashTextInfo;
        flashText.text = flashTextInfo;

        flashTime = setFlashTime;
        
        appearFlashTrigger = false;
    }

    private void AppearConditionedTrigger()
    {
        if(flashTextObject.gameObject.activeSelf)
            return;
        
        conditionedTextObject.SetActive(true);
        
        History.text += conditionedTextInfo;
        conditionedText.text = conditionedTextInfo;

        appearConditionedTrigger = false;
    }

    private void FixedUpdate()
    {
        flashTime -= Time.fixedDeltaTime;
        
        if (flashTime < 0)
        {
            flashTextObject.SetActive(false);
        }
    }
    
    void OnEnable()
    {
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 场景加载完成时的回调函数
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex>0){
            hisObj.SetActive(true);
            History.text = "  ";
        }
    }
}
