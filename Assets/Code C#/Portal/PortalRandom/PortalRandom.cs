//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class PortalRandom : MonoBehaviour
//{
//    public List<Transform> destinations; // Danh sách các điểm đến
//    public Image screenOverlay; // Reference đến hình ảnh overlay để làm tối màn hình
//    public AudioClip teleportSound;

//    private bool isTeleporting = false; // Biến để kiểm tra xem có đang trong quá trình teleport hay không

//    private void Awake()
//    {
//        // Khởi tạo danh sách các điểm đến (nếu chưa được khởi tạo)
//        if (destinations == null)
//        {
//            destinations = new List<Transform>();
//        }

//        // Thêm các điểm đến từ các child object của cổng
//        foreach (Transform child in transform)
//        {
//            if (child.CompareTag("Destination"))
//            {
//                destinations.Add(child);
//            }
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player") && !isTeleporting)
//        {
//            // Kiểm tra khoảng cách để tránh việc bắt đầu teleport ngay khi va chạm
//            if (Vector2.Distance(collision.transform.position, transform.position) > 0.3f)
//            {
//                StartCoroutine(PortalIn(collision.transform));
//            }
//        }
//    }

//    IEnumerator PortalIn(Transform playerTransform)
//    {
//        isTeleporting = true; // Đánh dấu đang trong quá trình teleport

//        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
//        Animator anim = playerTransform.GetComponent<Animator>();

//        // Tắt vật lý của player và phát animation Tele
//        playerRb.simulated = false;
//        anim.Play("PoIn");

//        // Phát âm thanh khi bắt đầu teleport
//        AudioSource.PlayClipAtPoint(teleportSound, playerTransform.position);

//        // Làm mờ màn hình
//        yield return StartCoroutine(FadeScreen(true));

//        if (destinations.Count > 0)
//        {
//            // Chọn ngẫu nhiên một điểm đến từ danh sách
//            Transform randomDestination = destinations[Random.Range(0, destinations.Count)];

//            // Di chuyển player đến vị trí đích
//            playerTransform.position = randomDestination.position;
//        }
//        else
//        {
//            Debug.LogError("Không có điểm đến nào được định nghĩa cho cổng!");
//        }

//        // Phát animation TeleOut
//        anim.Play("PoOut");

//        // Làm sáng màn hình
//        yield return StartCoroutine(FadeScreen(false));

//        // Bật lại vật lý cho player
//        playerRb.simulated = true;

//        // Đảm bảo hình ảnh overlay đã biến mất sau khi hoàn tất
//        screenOverlay.color = new Color(0, 0, 0, 0);

//        isTeleporting = false; // Kết thúc quá trình teleport
//    }

//    IEnumerator FadeScreen(bool darken)
//    {
//        Color startColor = darken ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1); // Màu ban đầu
//        Color endColor = darken ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0); // Màu cuối cùng

//        float duration = 0.5f; // Thời gian làm mờ hoặc làm sáng màn hình
//        float startTime = Time.time;

//        while (Time.time < startTime + duration)
//        {
//            float t = (Time.time - startTime) / duration;
//            screenOverlay.color = Color.Lerp(startColor, endColor, t);
//            yield return null;
//        }

//        screenOverlay.color = endColor; // Đảm bảo giá trị cuối cùng là màu cuối cùng
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PortalRandom : MonoBehaviour
{
    public Tilemap tilemap; // Tham chiếu đến Tilemap chứa các ô làm điểm đến
    public Image screenOverlay; // Tham chiếu đến hình ảnh overlay để làm tối màn hình
    public AudioClip teleportSound; // Âm thanh khi teleport

    private bool isTeleporting = false; // Biến để kiểm tra xem có đang trong quá trình teleport hay không

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm với đối tượng có tag là "Player" và không đang trong quá trình teleport
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            // Bắt đầu coroutine PortalIn với đối số là transform của player
            StartCoroutine(PortalIn(collision.transform));
        }
    }

    IEnumerator PortalIn(Transform playerTransform)
    {
        isTeleporting = true; // Đánh dấu là đang trong quá trình teleport

        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>(); // Lấy Rigidbody2D của player
        Animator anim = playerTransform.GetComponent<Animator>(); // Lấy Animator của player

        // Hút nhân vật vào cổng
        while (Vector2.Distance(playerTransform.position, transform.position) > 0.1f)
        {
            playerTransform.position = Vector2.MoveTowards(playerTransform.position, transform.position, 5f * Time.deltaTime);
            yield return null;
        }

        // Tắt vật lý của player và phát animation "PoIn"
        playerRb.simulated = false;
        anim.Play("PoIn");

        // Phát âm thanh khi bắt đầu teleport tại vị trí của player
        AudioSource.PlayClipAtPoint(teleportSound, playerTransform.position);

        // Làm mờ màn hình bằng cách gọi coroutine FadeScreen với tham số là true (làm tối màn hình)
        yield return StartCoroutine(FadeScreen(true));

        // Chọn ngẫu nhiên một vị trí ô từ Tilemap làm điểm đến
        Vector3Int randomTilePosition = GetRandomTilePosition();
        if (randomTilePosition != Vector3Int.zero)
        {
            // Di chuyển player đến vị trí thế giới tương ứng với ô tile được chọn
            playerTransform.position = tilemap.CellToWorld(randomTilePosition);
        }
        else
        {
            // Nếu không có ô nào trên Tilemap, xuất thông báo lỗi ra Console
            Debug.LogError("Không có điểm đến nào được định nghĩa trên Tilemap!");
        }

        // Phát animation "PoOut" để kết thúc quá trình teleport
        anim.Play("PoOut");

        // Làm sáng màn hình bằng cách gọi coroutine FadeScreen với tham số là false (làm sáng màn hình)
        yield return StartCoroutine(FadeScreen(false));

        // Bật lại vật lý cho player
        playerRb.simulated = true;

        // Đảm bảo hình ảnh overlay đã biến mất sau khi hoàn tất quá trình teleport
        screenOverlay.color = new Color(0, 0, 0, 0);

        isTeleporting = false; // Đánh dấu kết thúc quá trình teleport
    }

    // Coroutine để làm mờ hoặc làm sáng màn hình
    IEnumerator FadeScreen(bool darken)
    {
        // Màu ban đầu của overlay
        Color startColor = darken ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);

        // Màu cuối cùng của overlay
        Color endColor = darken ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0);

        float duration = 0.5f; // Thời gian để làm mờ hoặc làm sáng màn hình
        float startTime = Time.time; // Thời điểm bắt đầu coroutine

        // Lặp để thay đổi alpha của overlay dần dần từ startColor đến endColor trong khoảng thời gian duration
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration; // Tính toán thời gian đã qua
            screenOverlay.color = Color.Lerp(startColor, endColor, t); // Thay đổi màu của overlay
            yield return null; // Chờ cho đến khi kết thúc frame hiện tại
        }

        screenOverlay.color = endColor; // Đảm bảo giá trị cuối cùng của overlay là màu cuối cùng
    }

    // Phương thức để lấy ngẫu nhiên một vị trí ô trên Tilemap
    Vector3Int GetRandomTilePosition()
    {
        List<Vector3Int> tilePositions = new List<Vector3Int>(); // Danh sách các vị trí ô trên Tilemap

        BoundsInt bounds = tilemap.cellBounds; // Lấy các giới hạn ô của Tilemap
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds); // Lấy tất cả các ô trên Tilemap

        // Duyệt qua từng ô trong Tilemap
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0); // Tạo vị trí ô trong Tilemap
                if (tilemap.GetTile(pos) != null)
                {
                    tilePositions.Add(pos); // Nếu có ô tại vị trí pos, thêm vị trí này vào danh sách
                }
            }
        }

        // Chọn ngẫu nhiên một vị trí từ danh sách các vị trí có ô
        if (tilePositions.Count > 0)
        {
            return tilePositions[Random.Range(0, tilePositions.Count)];
        }
        else
        {
            return Vector3Int.zero; // Trả về vị trí (0, 0, 0) nếu không có ô nào trên Tilemap
        }
    }
}

