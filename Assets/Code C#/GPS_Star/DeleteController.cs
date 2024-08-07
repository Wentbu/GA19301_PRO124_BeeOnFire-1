using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Gọi hàm DestroyAfterTime sau 10 giây
        Invoke("DestroyAfterTime", 15f);
    }

    // Hàm để hủy đối tượng sau một khoảng thời gian
    void DestroyAfterTime()
    {
        Destroy(gameObject);
    }

    // Khi một collider khác chạm vào collider của dấu chân
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu collider chạm vào player có tag "Player"
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Hủy đối tượng dấu chân
        }
    }
}
