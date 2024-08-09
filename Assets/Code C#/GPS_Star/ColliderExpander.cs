using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderExpander : MonoBehaviour
{
    public float expansionRate = 1.0f; // Tốc độ tăng kích thước của collider
    public float maxRadius = 5.0f; // Kích thước tối đa của collider
    private CircleCollider2D circleCollider; // Collider loại CircleCollider2D
    private bool isExpanding = true; // Biến để kiểm tra xem collider có đang được mở rộng hay không

    private Vector3 firstBookPosition; // Vị trí của quyển sách đầu tiên được chạm vào

    void Start()
    {
        // Lấy component CircleCollider2D của đối tượng
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (isExpanding && circleCollider.radius < maxRadius)
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
        // Kiểm tra nếu collider va chạm với vật phẩm có tag "Sach" và đang mở rộng
        if (other.CompareTag("Book") && isExpanding)
        {
            firstBookPosition = other.transform.position;
            isExpanding = false; // Dừng việc mở rộng collider
            Debug.Log("Đã chạm vào vật phẩm 'Sach', dừng mở rộng collider!"); 
        }
    }

    // Phương thức public để trả về vị trí của quyển sách đầu tiên
    public Vector3 GetFirstBookPosition()
    {
        return firstBookPosition;
    }
}
