using TMPro;
using UnityEngine;

public class inputLogin : MonoBehaviour
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
            LoginWeb.Instance.registerStatus.text = "Mật khẩu không trùng khớp";
        }
    }
}
