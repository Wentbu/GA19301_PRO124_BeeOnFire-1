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
    [SerializeField] private GamePlay gameplayData;
    public void playGame()
    {
        if (gamePlayData.login == true)
        {
            gameplayData.levelId = UnityEngine.Random.Range(1, 3);
            SceneManager.LoadScene(gamePlayData.levelId);
        }
        else
        {
            panelLogin.gameObject.SetActive(true);
        }
    }

    public void logOut()
    {
        gameplayData.login = false;
        gameplayData.userId = 0;
        gameplayData.UserName = string.Empty;
        gameplayData.PassWord = string.Empty;
    }
    public void exit()
    {
        Application.Quit();
    }

    public void GamePlay()
    {
        GameManager.Instance.LogGamePlay(gameplayData.starTime, gameplayData.endTime);
    }
    public void GetData()
    {
        LoginWeb.Instance.GetData();
    }
}
