using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePlay", menuName = "ScriptableObjects/DataGameplay")]
public class GamePlay : ScriptableObject
{
    public string UserName;
    public string PassWord;

    public int levelId;
    public int userId;
    public DateTime starTime;
    public DateTime endTime;
    public double playedTime;
    public bool login = false;
}
