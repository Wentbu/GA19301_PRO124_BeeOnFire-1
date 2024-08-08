using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 0;

    public HealthUI healthBar;
    public int Heal;
    public int Damages;


    public GameObject NB;
    public GameObject BB;
    public GameObject Exp;

    private void Awake()
    {
        healthBar.SetMaxHealth();
    }

    void HealtPlus(int Heal)
    {
        currentHealth += Heal;
        healthBar.SetHealth(currentHealth);
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void TakeDamage(int Damages)
    {
        currentHealth -= Damages;
        healthBar.SetHealth(currentHealth);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "books")
        {
            HealtPlus(Heal);
            Destroy(collision.gameObject);
            Instantiate(NB, transform.position, Quaternion.identity);
            Instantiate(Exp, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.tag == "danger")
        {
            TakeDamage(Damages);
            Destroy(collision.gameObject);
            Instantiate(BB, transform.position, Quaternion.identity);
            Instantiate(Exp, transform.position, Quaternion.identity);
        }

    }
}
