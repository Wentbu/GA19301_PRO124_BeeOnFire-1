using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float expansionRate; // Tốc độ mở rộng của collider
    public float maxRadius; // Kích thước tối đa của collider
    private CircleCollider2D circleCollider; // Collider loại CircleCollider2D
    private bool hasCollidedWithHealth = false; // Biến để kiểm tra nếu đã chạm vào vật phẩm sách
    private AiController aiController; // Thêm biến để giữ AiController
    private bool hasCollidedWithGPS = false; // Biến để kiểm tra nếu đã chạm vào vật phẩm GPS

    void Awake()
    {
        // Lấy component collider
        circleCollider = GetComponent<CircleCollider2D>();
        // Khởi đầu ban đầu
        circleCollider.radius = 0.5f;
    }

    void Update()
    {
        if (circleCollider != null && hasCollidedWithGPS && !hasCollidedWithHealth)
        {
            // Mở rộng collider theo thời gian
            circleCollider.radius += expansionRate * Time.deltaTime;
            // Đảm bảo kích thước không vượt quá maxRadius
            if (circleCollider.radius > maxRadius)
            {
                circleCollider.radius = maxRadius;
            }
            Debug.Log("Đã tăng collider");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu collider chạm vào vật phẩm có tag "GPS"
        if (other.CompareTag("GPS") && !hasCollidedWithGPS)
        {
            hasCollidedWithGPS = true; // Đánh dấu là đã chạm vào vật phẩm GPS
            Debug.Log("Nhặt được vật phẩm GPS! Bắt đầu mở rộng collider.");
        }

        // Kiểm tra nếu collider chạm vào vật phẩm có tag "Sach"
        if (other.CompareTag("Sach") && hasCollidedWithGPS && !hasCollidedWithHealth)
        {
            hasCollidedWithHealth = true; // Đánh dấu là đã chạm vào vật phẩm sách
            Debug.Log("Chạm vào vật phẩm sách! Vị trí: " + other.transform.position); // Thông báo và in ra vị trí

            // Dừng việc mở rộng collider khi đã chạm vào sách
            expansionRate = 0f;

            // Cập nhật mục tiêu của AiController
            if (aiController != null)
            {
                aiController.SetTarget(other.transform);
            }
        }
    }

    // Phương thức để gán AiController từ bên ngoài
    public void SetAiController(AiController aiController)
    {
        this.aiController = aiController;
    }
}
