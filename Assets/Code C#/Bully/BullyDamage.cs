using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullyDamage : MonoBehaviour
{
    private bool isPlayerTouching = false;
    private PlayerHealth playerHealth;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerTouching = true;
            playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            StartCoroutine(DamageOverTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerTouching = false;
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (isPlayerTouching && playerHealth != null)
        {
            playerHealth.ApplyDamage();
            yield return null;
        }
    }
}
