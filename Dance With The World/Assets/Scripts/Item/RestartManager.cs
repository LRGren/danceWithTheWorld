using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //print(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            GameManager.instance.TP(Vector3.zero);
        }
    }
}
