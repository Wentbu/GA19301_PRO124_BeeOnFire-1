using UnityEngine;

[CreateAssetMenu(fileName = "MiniGame", menuName = "ScriptableObjects/MiniGameData")]
public class MiniGame : ScriptableObject
{
    public int gameplayId;
    public int userId;
    public int CompetitorId;
    public string result;
    public int score;
}
