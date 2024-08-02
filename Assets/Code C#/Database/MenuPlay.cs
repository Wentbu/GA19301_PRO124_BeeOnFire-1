using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPlay : MonoBehaviour
{
    public GamePlay gamePlayData;
    public GameObject panelLogin;
    public Button[] level;
    public void GetLevel1()
    {
        gamePlayData.levelId = level.Length +1;
    }

    public void playGame()
    {
        if (gamePlayData.login)
        {
            SceneManager.LoadScene(gamePlayData.levelId);
        }
        else
        {
            panelLogin.gameObject.SetActive(true);
        }
    }
}
