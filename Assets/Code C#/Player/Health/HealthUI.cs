using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    public void SetMaxHealth()
    {
        slider.maxValue = 100;
        slider.value = 0;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
