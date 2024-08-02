using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager>
{
    private static readonly string SaveGameplayURL = "https://phamduchuan.name.vn/SaveGameTime.php";

    public void LogGamePlay(DateTime startTime, DateTime endTime)
    {
        double playedTime = Math.Floor((endTime - startTime).TotalSeconds);
        StartCoroutine(SendGamePlayData(startTime, endTime, playedTime));
    }

    private IEnumerator SendGamePlayData(DateTime startTime, DateTime endTime, double playedTime)
    {
        WWWForm form = new WWWForm();
        form.AddField("Levels_Id", 1);
        form.AddField("User_Id", input.Instance.userid);
        form.AddField("Start_Time", startTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("End_Time", endTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("Played_Time", playedTime.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(SaveGameplayURL, form))
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
}
