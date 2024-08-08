using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlay : MonoBehaviour
{
    [SerializeField] private GamePlay gamePlayData;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject groupButtonMenu;
    [SerializeField] private GamePlay gameplayData;
    public void playGame()
    {
        if (gamePlayData.login == true)
        {
            gameplayData.levelId = Random.Range(1, 4);
            SceneManager.LoadScene(gamePlayData.levelId);
        }
        else
        {
            panelLogin.gameObject.SetActive(true);
            groupButtonMenu.gameObject.SetActive(false);
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

    public void GetData()
    {
        LoginWeb.Instance.GetData();
    }
}
