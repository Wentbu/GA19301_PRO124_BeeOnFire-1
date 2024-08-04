using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GamePlay", menuName ="DataGameplay")]
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
