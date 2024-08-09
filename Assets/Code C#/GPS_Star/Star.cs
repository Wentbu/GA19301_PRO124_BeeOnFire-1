using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject footprintPrefab;     // Prefab của dấu chân
    public float footprintSpacing = 0.5f;  // Khoảng cách giữa các dấu chân
    private Vector3 lastFootprintPosition; // Vị trí của dấu chân cuối cùng
    private Vector3 initialPosition;       // Vị trí ban đầu của AI

    private AiController aiController;    // Tham chiếu đến script AiController

    void Start()
    {
        initialPosition = transform.position; // Lưu vị trí ban đầu của AI
        lastFootprintPosition = initialPosition;
        aiController = FindObjectOfType<AiController>(); // Tìm đối tượng AiController trong Scene

        // Thiết lập mục tiêu ban đầu cho AI là sách
        GameObject sachObject = GameObject.FindWithTag("Book");
        if (sachObject != null)
        {
            aiController.SetTarget(sachObject.transform);
        }
    }

    void Update()
    {
        // Kiểm tra khoảng cách giữa các dấu chân
        if (Vector3.Distance(transform.position, lastFootprintPosition) > footprintSpacing)
        {
            CreateFootprint(); // Tạo dấu chân nếu khoảng cách giữa các dấu chân lớn hơn khoảng cách quy định
            lastFootprintPosition = transform.position; // Cập nhật lại vị trí dấu chân cuối cùng
        }
    }

    void CreateFootprint()
    {
        // Tạo dấu chân tại vị trí hiện tại của nhân vật
        Instantiate(footprintPrefab, transform.position, Quaternion.identity);
    }
}
