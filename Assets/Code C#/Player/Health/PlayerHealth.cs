using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("PlayerHealth")]
    private int currentHealth;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthUI healthBar;
    [SerializeField] private int healAmount;
    [SerializeField] private int damageAmount;

    [SerializeField] private GameObject normalBookEffect;
    [SerializeField] private GameObject badBookEffect;
    [SerializeField] private GameObject explosionEffect;

    public static event Action<float> OnHealthChanged;


    private void Awake()
    {
        currentHealth = 0;
        healthBar.Initialize(maxHealth, currentHealth);
        healthBar.OnHealthChanged += HandleHealthUIChanged;
    }

    private void OnDestroy()
    {
        if (healthBar != null)
        {
            healthBar.OnHealthChanged -= HandleHealthUIChanged;
        }
    }

    private void HandleHealthUIChanged(float healthPercentage)
    {
        OnHealthChanged?.Invoke(healthPercentage);

        if (healthBar.IsCriticalHealth())
        {
            Debug.Log("Cảnh báo: Sức khỏe người chơi rất thấp!");
            // Thêm logic xử lý khi sức khỏe ở mức nguy hiểm
        }

        if (healthBar.IsFullHealth())
        {
            Debug.Log("Người chơi đã hồi phục hoàn toàn!");
            // Thêm logic xử lý khi sức khỏe đầy
        }
    }

    private void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(this);
        }
    }

    public void ApplyHeal() => ModifyHealth(healAmount);
    public void ApplyDamage() => ModifyHealth(-damageAmount);

    public void SpawnEffect(EffectType effectType)
    {
        GameObject effectPrefab = effectType switch
        {
            EffectType.NormalBook => normalBookEffect,
            EffectType.BadBook => badBookEffect,
            EffectType.Explosion => explosionEffect,
            _ => null
        };

        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
    }
}

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

public class Book : MonoBehaviour, IInteractable
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