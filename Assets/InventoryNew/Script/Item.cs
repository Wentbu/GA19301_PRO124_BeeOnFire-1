using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    [Header("Gameplay")]
    public int ItemID ;
    public ActionType actType;
    public Vector2Int range = new Vector2Int(5, 4);
    public bool stackable = true;

    [Header("UI")]
    public Sprite image;

    // New method to use the item
    public virtual void Use(PlayerControl player)
    {
        // Default implementation, can be overridden by derived classes
    }
}



public enum ActionType
{
    Condition,
    Usable
}
