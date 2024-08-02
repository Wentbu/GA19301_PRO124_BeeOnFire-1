using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI.Table;

public class LoginWeb : Singleton<LoginWeb>
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI Rank;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Time;

    [SerializeField] TextMeshProUGUI dangNhap;
    [SerializeField] TextMeshProUGUI dangKy;

    private static readonly string LoginURL = "https://phamduchuan.name.vn/LogIn.php";
    private static readonly string RegisterURL = "https://phamduchuan.name.vn/RegisterUser.php";
    private static readonly string rankURL = "https://phamduchuan.name.vn/rank.php";
    private static readonly string GetItemValueURL = "https://phamduchuan.name.vn/GetValueItems.php";

    public GameObject[] rows;
    private void Start()
    {
        StartCoroutine(GetItems());
    }

    public void login()
    {
        dangNhap.text = string.Empty;
        StartCoroutine(LogInCoroutine());
    }
    IEnumerator LogInCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("User_Name", input.Instance.GetUser());
        form.AddField("Password_", input.Instance.GetPasword());

        using (UnityWebRequest www = UnityWebRequest.Post(LoginURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                dangNhap.text = "L?i m?ng: " + www.error;
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                ProcessResponse(www.downloadHandler.text);
            }
        }
    }


    void ProcessResponse(string response)
    {
        Debug.Log(response);

        // Phân tích ph?n h?i JSON
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);

        if (responseData.status == "success")
        {
            dangNhap.text = "Đăng nhập thành công!";
            input.Instance.userid = responseData.User_Id;
            // Th?c hi?n các logic game khi ??ng nh?p thành công
        }
        else
        {
            dangNhap.text = "Đăng nhập thất bại: " + responseData.message;
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        public string status;
        public int User_Id;
        public string message;
    }


    public void registerUser()
    {
        dangKy.text = string.Empty;
        StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("User_Name", input.Instance.GetUser());
        form.AddField("Password_", input.Instance.GetPasword());

        using (UnityWebRequest www = UnityWebRequest.Post(RegisterURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                dangKy.text = "L?i m?ng: " + www.error;
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                ProcessResponseRegister(www.downloadHandler.text);
            }
        }
    }

    void ProcessResponseRegister(string response)
    {
        Debug.Log(response);

        // Phân tích ph?n h?i JSON
        var responseDataRegister = JsonUtility.FromJson<ResponseData>(response);

        if (responseDataRegister.status == "success")
        {
            dangKy.text = "??ng ký thành công! " + responseDataRegister.message;
        }
        else
        {
            dangKy.text = "??ng ký th?t b?i: " + responseDataRegister.message;
        }
    }

    [System.Serializable]
    public class ResponseDataRegister
    {
        public string status;
        public string message;
    }


    public void GetData()
    {
        Rank.text += string.Empty;
        Name.text += string.Empty;
        Time.text += string.Empty;
        StartCoroutine(GetDataFromServer());
    }
    IEnumerator GetDataFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(rankURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                Debug.Log("Received: " + jsonResult);

                try
                {
                    GameDataWrapper gameDataWrapper = JsonUtility.FromJson<GameDataWrapper>(jsonResult);

                    if (gameDataWrapper != null && gameDataWrapper.items.Length > 0)
                    {
                        for (int i = 0; i < gameDataWrapper.items.Length; i++)
                        {
                            if (i < rows.Length)
                            {
                                var data = gameDataWrapper.items[i];
                                TextMeshProUGUI[] texts = rows[i].GetComponentsInChildren<TextMeshProUGUI>();

                                texts[0].text = data.Ranking.ToString();
                                texts[1].text = data.Character_Name;
                                texts[2].text = data.Played_Time + "S";
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("No items found");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log("JSON parse error: " + ex.Message);
                }
            }
        }
    }
    [System.Serializable]
    private class GameData
    {
        public int Ranking;
        public string Character_Name;
        public string Played_Time;
    }

    [System.Serializable]
    private class GameDataWrapper
    {
        public GameData[] items;
    }


    public ItemValue itemList;
    IEnumerator GetItems()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetItemValueURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            List<ItemValue.itemValue> items = JsonConvert.DeserializeObject<List<ItemValue.itemValue>>(json);
            itemList.Itemp = items;
            foreach (ItemValue.itemValue item in items)
            {
                Debug.Log("Item ID: " + item.Items_Id + ", Name: " + item.Items_Name + ", Feature: " + item.Feature + ", Duration: " + item.Duration);
            }
        }
    }
}
