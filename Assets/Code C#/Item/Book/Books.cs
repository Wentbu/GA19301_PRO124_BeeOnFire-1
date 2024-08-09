using UnityEngine;

public enum EffectType
{
    NormalBook,
    BadBook,
    Explosion
}

public interface IInteractable
{
    void Interact(PlayerHealth healthManager);
}

public class Books : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isGoodBook;
    public void Interact(PlayerHealth healthManager)
    {
        if (isGoodBook)
        {
            healthManager.ApplyHeal();
            healthManager.SpawnEffect(EffectType.NormalBook);
        }
        else
        {
            healthManager.ApplyDamage();
            healthManager.SpawnEffect(EffectType.BadBook);
        }

        healthManager.SpawnEffect(EffectType.Explosion);
        Destroy(gameObject);
    }
}