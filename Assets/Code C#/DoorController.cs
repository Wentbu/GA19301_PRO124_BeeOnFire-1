using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E; // Phím để tương tác với cửa
    public Animator doorAnimator; // Animator để điều khiển animation mở cửa
    public string nextSceneName; // Tên của scene tiếp theo

    private bool isPlayerNearby = false; // Biến kiểm tra xem người chơi có gần cửa hay không

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void OpenDoor()
    {
        doorAnimator.SetTrigger("Open"); // Kích hoạt trigger "Open" trong animator
        Invoke("ChangeScene", 1f); // Gọi hàm ChangeScene sau 1 giây
    }

    private void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName); // Chuyển đổi sang scene tiếp theo
    }
}
