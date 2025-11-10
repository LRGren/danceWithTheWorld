using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopManager : MonoBehaviour
{
    public static StopManager Instance;

    public MainUI MainUI;
    
    private bool isStopped = false;  //这个变量用来标记游戏是否处于暂停状态
    public bool IsStopped { get { return isStopped; } } //这是get方法
    Button returnGameBt;
    Button quitBt;
    Button skillsBt;
    bool isSkillsmenuopen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).Find("Skillsmenu").gameObject.SetActive(false);

        returnGameBt = transform.GetChild(0).Find("ReturnGame").GetComponent<Button>();
        quitBt = transform.GetChild(0).Find("QuitBt").GetComponent<Button>();
        skillsBt = transform.GetChild(0).Find("Skills").GetComponent<Button>();

        returnGameBt.onClick.AddListener(ReturnGame);
        quitBt.onClick.AddListener(QuitGame);
        skillsBt.onClick.AddListener(Skills);
    }

    void Update() 
    {
        if(MainUI!=null)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isStopped && isSkillsmenuopen)
            {
                isSkillsmenuopen = false;
                transform.GetChild(0).Find("Skillsmenu").gameObject.SetActive(false);
            }
            else if (isStopped)
            {
                ReturnGame();
            }
            else
            {
                StopGame();
            }
        }
    }

    public void StopGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        isStopped = true;
        Time.timeScale = 0f;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game from Stop Menu");
    }
    void ReturnGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isStopped = false;
        Time.timeScale = 1f;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    void Skills()
    {
        isSkillsmenuopen = true;
        transform.GetChild(0).Find("Skillsmenu").gameObject.SetActive(true);
        Debug.Log("Open Skills Menu");
    }
}
