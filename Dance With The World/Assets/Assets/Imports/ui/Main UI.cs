using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
public class MainUI : MonoBehaviour
{
    Button newGameBt;
    Button continueBt;
    Button quitBt;

    void Awake() 
    {
        newGameBt = transform.GetChild(0).Find("NewGameBt").GetComponent<Button>();
        //continueBt = transform.Find("ContinueBt").GetComponent<Button>();
        quitBt = transform.GetChild(0).Find("QuitBt").GetComponent<Button>();

        newGameBt.onClick.AddListener(NewGameBt);
        //continueBt.onClick.AddListener(ContinueBt);
        quitBt.onClick.AddListener(QuitGame);
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void Update()
    {

    }


    void NewGameBt()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.instance.LoadScene(1);
    }

    void ContinueBt()
    {

        Debug.Log("Continue Game");
    }
    void QuitGame()
    {
        Application.Quit();
        //Debug.Log("Quit Game");
    }
}
