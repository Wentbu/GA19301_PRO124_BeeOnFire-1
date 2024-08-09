using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;

    public event Action<float> OnHealthChanged;

    private void Awake()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (slider == null)
            Debug.LogError("Slider is not assigned in HealthUI!");
        if (fillImage == null)
            Debug.LogError("Fill Image is not assigned in HealthUI!");
    }

    public void Initialize(int maxHealth, int currentHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            SetHealth(currentHealth);
        }
    }

    public void SetHealth(int health)
    {
        if (slider != null)
        {
            slider.value = health;
            UpdateFillColor(GetHealthPercentage());
            OnHealthChanged?.Invoke(GetHealthPercentage());
        }
    }

    public void ModifyHealth(int amount)
    {
        if (slider != null)
        {
            SetHealth(Mathf.Clamp((int)slider.value + amount, 0, (int)slider.maxValue));
        }
    }

    private float GetHealthPercentage()
    {
        return slider != null ? slider.value / slider.maxValue : 0f;
    }

    private void UpdateFillColor(float percentage)
    {
        if (fillImage != null && healthGradient != null)
        {
            fillImage.color = healthGradient.Evaluate(percentage);
        }
    }

    public bool IsFullHealth() => Mathf.Approximately(GetHealthPercentage(), 1f);

    public bool IsCriticalHealth() => GetHealthPercentage() <= 0.2f;
}