using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int initialHealth = 0;
    [SerializeField] private HealthUI healthBar;

    [Header("Heal Settings")]
    [SerializeField] private int healAmount = 10;
    [SerializeField] private float healCooldown = 1f;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject normalBookEffect;
    [SerializeField] private GameObject badBookEffect;
    [SerializeField] private GameObject explosionEffect;

    public static event Action<float> OnHealthChanged;

    private int currentHealth;
    private float lastHealTime;
    private float lastDamageTime;

    private void Awake()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        currentHealth = Mathf.Clamp(initialHealth, 0, maxHealth);
        healthBar.Initialize(maxHealth, currentHealth);
        healthBar.OnHealthChanged += HandleHealthUIChanged;
    }

    private void OnDestroy()
    {
        healthBar.OnHealthChanged -= HandleHealthUIChanged;
    }

    private void HandleHealthUIChanged(float healthPercentage)
    {
        OnHealthChanged?.Invoke(healthPercentage);

        if (healthBar.IsCriticalHealth())
        {
            Debug.Log("⚠️ Cảnh báo: Sức khỏe người chơi rất thấp!");
        }
        else if (healthBar.IsFullHealth())
        {
            Debug.Log("🎉 Người chơi đã hồi phục hoàn toàn!");
        }
    }

    public void ApplyHeal()
    {
        if (Time.time - lastHealTime < healCooldown) return;

        int actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
        if (actualHeal > 0)
        {
            ModifyHealth(actualHeal);
            lastHealTime = Time.time;
            Debug.Log($"💚 Người chơi được hồi {actualHeal} máu. Máu hiện tại: {currentHealth}/{maxHealth}");
        }
        else
        {
            Debug.Log("🛑 Không thể hồi máu, sức khỏe đã đầy!");
        }
    }

    public void ApplyDamage()
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        ModifyHealth(-damageAmount);
        lastDamageTime = Time.time;
        Debug.Log($"❤️ Người chơi nhận {damageAmount} sát thương. Máu hiện tại: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }

    private void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(this);
        }
    }

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