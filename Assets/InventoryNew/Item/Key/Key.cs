using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/KeyItem")]
public class Key : Item
{
    [SerializeField] public string keyCode;
}
