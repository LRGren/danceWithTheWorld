using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerManager playerManager;

    [Header("Player States")]
    public bool isDancingMode;
    public bool isUI;
    
    [Header("Loading")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public TextMeshProUGUI  loadingText;
    
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
        
        loadingScreen.SetActive(false);
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        DontDestroyOnLoad(this);
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(loadLevel(sceneIndex));
    }

    public void TP(Vector3 targetPosition)
    {
        playerManager.characterController.enabled = false;
        playerManager.transform.position = targetPosition;
        playerManager.characterController.enabled = true;
    }

    IEnumerator loadLevel(int index)
    {
        loadingScreen.SetActive(true);
        
        playerManager.characterController.enabled = false;
        playerManager.transform.position = Vector3.zero;
        playerManager.characterController.enabled = true;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingSlider.value = operation.progress;

            loadingSlider.value = 0.5f * Time.time;
            loadingText.text = Mathf.FloorToInt(loadingSlider.value * 100f).ToString() + "%";

            if (operation.progress >= 0.9f)
            {
                loadingSlider.value = 1;
                
                loadingText.text = "Press any key to continue...";

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            
            yield return null;
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
        loadingScreen.SetActive(false);
    }
}
