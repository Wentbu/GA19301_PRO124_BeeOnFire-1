using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPlay : MonoBehaviour
{
    [SerializeField] private GamePlay gamePlayData;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private Button[] level;
    public void GetLevel()
    {
        gamePlayData.levelId = level.Length +1;
    }

    public void playGame()
    {
        if (gamePlayData.login == true)
        {
            SceneManager.LoadScene(gamePlayData.levelId);
            gamePlayData.starTime = DateTime.Now;
        }
        else
        {
            panelLogin.gameObject.SetActive(true);
        }
    }
}
