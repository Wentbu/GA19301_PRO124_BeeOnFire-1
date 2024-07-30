using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class input : Singleton<input>
{
    public string user;
    public string password;
    public string password2;
    public int userid;

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
