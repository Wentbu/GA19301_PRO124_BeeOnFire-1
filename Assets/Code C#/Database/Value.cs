using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class Value : MonoBehaviour
{
    [SerializeField] private TMP_InputField userLogin;
    [SerializeField] private TMP_InputField passwordLogin;
    [SerializeField] private TMP_InputField userRegister;
    [SerializeField] private TMP_InputField passwordRegister;
    [SerializeField] private TMP_InputField passwordRegister2;

    private DateTime star;
    private DateTime end;
    private void Start()
    {
        star = DateTime.Now;
    }

    public void GetValueLogin()
    {
        if (userLogin.text.Trim().Length == 0) return;
        input.Instance.value(userLogin.text, passwordLogin.text, passwordRegister2.text);
    }
    public void GetValueRegister()
    {
        if (userRegister.text.Trim().Length == 0) return;
        input.Instance.value(userRegister.text, passwordRegister.text, passwordRegister2.text);
    }

    public void LogIn()
    {
        LoginWeb.Instance.login();
    }
    public void RegisterUser()
    {
        LoginWeb.Instance.registerUser();
        //if (input.Instance.GetPasword() == input.Instance.GetPassword2())
        //{
        //    Web.Instance.registerUser(input.Instance.GetUser(), input.Instance.GetPasword());
        //}else
        //{
        //    Debug.Log("M?t kh?u không trùng kh?p");
        //}
    }
    public void GamePlay()
    {
        end = DateTime.Now;
        LoginWeb.Instance.LogGamePlay(star, end);
    }
    public void GetData()
    {
        LoginWeb.Instance.GetData();
    }
}
