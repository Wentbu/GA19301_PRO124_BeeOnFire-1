using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderExpander : MonoBehaviour
{
    public float expansionRate = 1.0f; // Tốc độ tăng kích thước của collider
    public float maxRadius = 5.0f; // Kích thước tối đa của collider
    private CircleCollider2D circleCollider; // Collider loại CircleCollider2D
    private bool isExpanding = true; // Biến để kiểm tra xem collider có đang được mở rộng hay không

    private bool hasCollidedWithGPS = false;
    private bool hasCollidedWithHealth = false;

    private Vector3 firstBookPosition; // Vị trí của quyển sách đầu tiên được chạm vào
    private AiController aiController; // Tham chiếu đến AiController

    void Start()
    {
        // Lấy component CircleCollider2D của đối tượng
        circleCollider = GetComponent<CircleCollider2D>();

        if (circleCollider == null)
        {
            Debug.LogError("Không tìm thấy CircleCollider2D trên đối tượng!");
        }
    }

    void Update()
    {
        if (isExpanding && circleCollider != null && circleCollider.radius < maxRadius)
        {
            // Tăng kích thước của collider dần theo thời gian
            circleCollider.radius += expansionRate * Time.deltaTime;
            // Đảm bảo kích thước không vượt quá maxRadius
            if (circleCollider.radius > maxRadius)
            {
                circleCollider.radius = maxRadius;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("GPS"))
        //{
        //    hasCollidedWithGPS = true;
        //    Debug.Log("Chạm vào GPS! Vị trí: " + other.transform.position);
        //}

        if (other.CompareTag("Book") && hasCollidedWithGPS && !hasCollidedWithHealth)
        {
            hasCollidedWithHealth = true; // Đánh dấu là đã chạm vào vật phẩm sách
            firstBookPosition = other.transform.position; // Cập nhật vị trí của quyển sách đầu tiên
            Debug.Log("Chạm vào vật phẩm sách! Vị trí: " + firstBookPosition); // Thông báo và in ra vị trí

            // Dừng việc mở rộng collider khi đã chạm vào sách
            expansionRate = 0f;

            // Cập nhật mục tiêu của AiController
            if (aiController != null)
            {
                aiController.SetTarget(other.transform);
            }

            // Vô hiệu hóa và kích hoạt lại collider sách
            StartCoroutine(DisableCollisionTemporarily(other));
        }
    }

    private IEnumerator DisableCollisionTemporarily(Collider2D bookCollider)
    {
        Debug.Log("Vô hiệu hóa collider sách");
        // Vô hiệu hóa collider của sách để tránh va chạm thêm
        bookCollider.enabled = false;
        // Chờ một thời gian ngắn (1 giây)
        yield return new WaitForSeconds(1.0f);
        // Kích hoạt lại collider của sách
        Debug.Log("Kích hoạt lại collider sách");
        bookCollider.enabled = true;
    }

    // Phương thức public để trả về vị trí của quyển sách đầu tiên
    public Vector3 GetFirstBookPosition()
    {
        return firstBookPosition;
    }

    // Phương thức để gán AiController từ bên ngoài
    public void SetAiController(AiController controller)
    {
        aiController = controller;
        Debug.Log("AiController đã được gán!");
    }
}
