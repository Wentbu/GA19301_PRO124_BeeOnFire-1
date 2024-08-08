using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemUmbrella : MonoBehaviour
{
    PrefabUmbrella prefabUmbrella;
    void Start()
    {
        prefabUmbrella = FindObjectOfType<PrefabUmbrella>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (prefabUmbrella != null)
            {
                prefabUmbrella.AttachUmbrellaToPlayer(collision.gameObject);
                Destroy(gameObject); // Hủy itemUmbrella sau khi nhặt
            }
        }
    }
}
