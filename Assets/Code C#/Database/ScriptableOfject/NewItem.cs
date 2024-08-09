using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/NewItem", order = 2)]
public class NewItem : ScriptableObject
{
    public int Items_Id;
    public string Items_Name;
    public string Feature;
    public double Duration;
}
