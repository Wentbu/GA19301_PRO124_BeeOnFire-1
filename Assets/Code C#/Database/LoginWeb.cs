using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.Windows;

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
    private static readonly string rank = "https://phamduchuan.name.vn/rank.php";
    private static readonly string SaveGameplay = "https://phamduchuan.name.vn/SaveGameTime.php";
    private static readonly string GetItemValue = "https://phamduchuan.name.vn/GetValueItems.php";


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
            dangNhap.text = "??ng nh?p thành công! Mã ng??i dùng: " + responseData.User_Id;
            input.Instance.userid = responseData.User_Id;
            // Th?c hi?n các logic game khi ??ng nh?p thành công
        }
        else
        {
            dangNhap.text = "??ng nh?p th?t b?i: " + responseData.message;
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

        using (UnityWebRequest www = UnityWebRequest.Get(rank))
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
                        string displayText = "";
                        foreach (var data in gameDataWrapper.items)
                        {
                            displayText += "STT: " + data.Ranking + " - Character_Name: " + data.Character_Name +
                                " - Played_Time: " + data.Played_Time + "\n";
                            Rank.text += data.Ranking + "\n";
                            Name.text += data.Character_Name + "\n";
                            Time.text += data.Played_Time + "S\n";
                        }
                    }
                    else
                    {
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

    public void LogGamePlay(DateTime startTime, DateTime endTime)
    {
        double playedTime = (endTime - startTime).TotalSeconds;
        StartCoroutine(SendGamePlayData(startTime, endTime, playedTime));
    }

    private IEnumerator SendGamePlayData(DateTime startTime, DateTime endTime, double playedTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("Level_Id", 1);
        form.AddField("Items_Id", 1);
        form.AddField("User_Id", input.Instance.userid);
        form.AddField("Start_Time", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("Ens_Time", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("Played_Time", playedTime.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(SaveGameplay, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    public ItemValue itemList;
    IEnumerator GetItems()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetItemValue);
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
