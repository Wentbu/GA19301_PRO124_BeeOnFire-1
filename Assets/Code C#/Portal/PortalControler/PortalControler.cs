using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Để sử dụng UI elements trong Unity

public class PortalControler : MonoBehaviour
{
    public Transform destination;
    GameObject player;
    Rigidbody2D playerRb;
    Animator anim;

    public Image screenOverlay; // Reference đến hình ảnh overlay để làm tối màn hình
    public AudioClip teleportSound;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        anim = player.GetComponent<Animator>();


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 0.3f)
            {
                StartCoroutine(PortalIn());
            }


        }
    }

    IEnumerator PortalIn()
    {
        // Tắt vật lý của player và phát animation Tele
        playerRb.simulated = false;
        anim.Play("Portal In");

        // Phát âm thanh khi bắt đầu teleport
        AudioSource.PlayClipAtPoint(teleportSound, player.transform.position);

        StartCoroutine(MoveInPortal());
        yield return new WaitForSeconds(0.5f);

        // Làm tối màn hình ngay lập tức và sau đó dần sáng lại
        StartCoroutine(FadeScreen(true)); // Làm tối màn hình ngay lập tức

        yield return new WaitForSeconds(2f); // Chờ 2 giây trước khi làm sáng màn hình

        StartCoroutine(FadeScreen(false)); // Làm sáng màn hình sau khi nhân vật hoàn tất di chuyển

        // Di chuyển player đến vị trí đích
        player.transform.position = destination.position;

        // Phát animation TeleOut
        anim.Play("Portal Out");
        yield return new WaitForSeconds(0.5f);

        // Bật lại vật lý cho player
        playerRb.simulated = true;

        // Đảm bảo hình ảnh overlay đã biến mất sau khi hoàn tất
        screenOverlay.color = new Color(0, 0, 0, 0);
    }


    IEnumerator MoveInPortal()
    {
        float time = 0;
        while (time < 0.5f)
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
    }

    IEnumerator FadeScreen(bool darken)
    {
        Color startColor = darken ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1); // Màu ban đầu
        Color endColor = darken ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0); // Màu cuối cùng

        float duration = 0.5f; // Thời gian làm tối hoặc làm sáng màn hình
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            screenOverlay.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        screenOverlay.color = endColor; // Đảm bảo giá trị cuối cùng là màu cuối cùng
    }
    
}