using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class input : MonoBehaviour
{


    [SerializeField] private TMP_InputField userLogin;
    [SerializeField] private TMP_InputField passwordLogin;
    [SerializeField] private TMP_InputField userRegister;
    [SerializeField] private TMP_InputField passwordRegister;
    [SerializeField] private TMP_InputField passwordRegister2;

    [SerializeField] private GamePlay dataGameplay;

    public void GetValueLogin()
    {
        if (userLogin.text.Trim().Length == 0) return;
        Value.Instance.value(userLogin.text, passwordLogin.text, passwordRegister2.text);
    }
    public void GetValueRegister()
    {
        if (userRegister.text.Trim().Length == 0) return;
        Value.Instance.value(userRegister.text, passwordRegister.text, passwordRegister2.text);
    }

    public void LogIn()
    {
        LoginWeb.Instance.login();
    }
    public void RegisterUser()
    {
        if (Value.Instance.GetPasword() == Value.Instance.GetPassword2())
        {
            LoginWeb.Instance.registerUser();
        }
        else
        {
            Debug.Log("M?t kh?u không trùng kh?p");
        }
    }
    public void GamePlay()
    {
        GameManager.Instance.LogGamePlay(dataGameplay.starTime, dataGameplay.endTime);
    }
    public void GetData()
    {
        LoginWeb.Instance.GetData();
    }
}
