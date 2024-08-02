using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class Value : Singleton<Value>
{
    [SerializeField] private string user;
    [SerializeField] private string password;
    [SerializeField] private string password2;

    public string GetUser()
    {
        return user;
    }

    public string GetPasword()
    {
        return password;
    }

    public string GetPassword2()
    {
        return password2;
    }
    public void value(string user, string password, string password2)
    {
        this.user = user;
        this.password = password;
        this.password2 = password2;
    }
}
