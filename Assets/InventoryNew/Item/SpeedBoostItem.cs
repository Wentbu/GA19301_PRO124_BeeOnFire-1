using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/SpeedBoostItem")]
public class SpeedBoostItem : Item
{
    [SerializeField] public float speedBoostAmount;
    [SerializeField] public float duration;

    public override void Use(PlayerControl playerController)
    {
        base.Use(playerController);
        playerController.ApplyBuffSpeed();
    }
}
