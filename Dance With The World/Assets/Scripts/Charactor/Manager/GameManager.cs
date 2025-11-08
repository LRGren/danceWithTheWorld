using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerManager playerManager;

    [Header("Player States")]
    public bool isDancingMode;
    
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
        playerManager = FindObjectOfType<PlayerManager>();
        DontDestroyOnLoad(this);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        
        playerManager.characterController.enabled = false;
        playerManager.transform.position = Vector3.zero;
        playerManager.characterController.enabled = true;
    }

    public void TP(Vector3 targetPosition)
    {
        playerManager.characterController.enabled = false;
        playerManager.transform.position = targetPosition;
        playerManager.characterController.enabled = true;
    }
}
