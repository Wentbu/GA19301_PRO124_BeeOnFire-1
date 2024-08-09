using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    [Header("Interface")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;

    [Header("Movement Settings")]
    private float moveSpeed;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float frictionAmount = 0.2f;

    [Header("NPC Interaction")]
    [SerializeField] private float interactDistance = 2f; //Khoảng cách để tương tác với npc (nếu cần)

    [Header("Buff Settings")]
    [SerializeField] private Coroutine buffSpeedCoroutine;
    [SerializeField] private Coroutine blinkCoroutine;
    [SerializeField] float buffDuration = 5f;
    [SerializeField] float blinkDuration = 2f;
    [SerializeField] Image buffSpeedIcon; // Icon cho buff
    [SerializeField] private GameObject shadowPrefab; // Prefab cho bóng mờ

    [Header("Debuff Settings")]
    [SerializeField] private float debuffDuration = 5f; // Thời gian giảm tốc độ
    [SerializeField] private float debuffPercentage = 0.5f; // Phần trăm giảm tốc độ (50%)
    [SerializeField] private Image debuffSpeedIcon; // Icon cho debuff
    private bool isDebuffed = false;
    private Coroutine debuffCoroutine;

    private Vector2 movement;
    private Vector2 smoothedMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        moveSpeed = maxSpeed;
    }

    private void FixedUpdate()
    {
        SmoothMovement();
        Move();
        ApplyFriction();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void SmoothMovement()
    {
        smoothedMovement = Vector2.Lerp(smoothedMovement, movement, Time.fixedDeltaTime * 10f);
    }

    private void Move()
    {
        // Giới hạn tốc độ tối đa
        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);

        float currentMaxSpeed = isDebuffed ? maxSpeed * debuffPercentage : maxSpeed;

        // Tính toán vận tốc tối đa
        Vector2 targetVelocity = smoothedMovement * currentMaxSpeed;

        // Tính toán vận tốc hiện tại và tối đa
        Vector2 velocityDiff = targetVelocity - rb2d.velocity;

        // Tính toán lực gia tốc
        Vector2 accelerationForce = velocityDiff * acceleration;

        // Áp dụng lực vào Rigidbody2D
        rb2d.AddForce(accelerationForce);
        rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, currentMaxSpeed);
    }

    private void ApplyFriction()
    {
        // Áp dụng ma sát khi người chơi bỏ tay ra khỏi nút bấm
        if (movement.magnitude < 0.01f)
        {
            float frictionForce = Mathf.Min(rb2d.velocity.magnitude, frictionAmount);
            rb2d.AddForce(-rb2d.velocity.normalized * frictionForce, ForceMode2D.Impulse);
        }
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Horizontal", smoothedMovement.x);
        animator.SetFloat("Vertical", smoothedMovement.y);
        animator.SetFloat("Speed", smoothedMovement.sqrMagnitude);
    }

    public void ApplyBuffSpeed()
    {
        if (buffSpeedCoroutine != null)
        {
            StopCoroutine(buffSpeedCoroutine);
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        buffSpeedCoroutine = StartCoroutine(BuffSpeedItemCoroutine());
    }

    private IEnumerator BuffSpeedItemCoroutine()
    {
        float buffSpeedIncrease = maxSpeed * 0.5f; // Tăng 50% so với tốc độ gốc
        moveSpeed = maxSpeed + buffSpeedIncrease;

        if (buffSpeedIcon != null)
        {
            buffSpeedIcon.gameObject.SetActive(true); // Hiển thị icon
        }

        StartCoroutine(CreateShadows(buffDuration)); // Bắt đầu tạo bóng mờ

        yield return new WaitForSeconds(buffDuration - blinkDuration); // Chờ buffDuration - blinkDuration giây

        if (buffSpeedIcon != null)
        {
            blinkCoroutine = StartCoroutine(BlinkIcon(buffSpeedIcon, blinkDuration)); // Bắt đầu nhấp nháy
        }

        yield return new WaitForSeconds(blinkDuration); // Chờ thêm blinkDuration giây

        ResetBuffSpeed();
    }

    private IEnumerator BlinkIcon(Image icon, float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            icon.gameObject.SetActive(!icon.gameObject.activeSelf); // Chuyển đổi trạng thái hiện/ẩn
            yield return new WaitForSeconds(0.2f); // Thời gian nhấp nháy
        }
        icon.gameObject.SetActive(false); // Ẩn icon khi kết thúc
    }

    private void ResetBuffSpeed()
    {
        moveSpeed = maxSpeed; // Khôi phục tốc độ di chuyển ban đầu
        if (buffSpeedIcon != null)
        {
            buffSpeedIcon.gameObject.SetActive(false); // Ẩn icon
        }

        buffSpeedCoroutine = null;
        blinkCoroutine = null;
    }

    private IEnumerator CreateShadows(float duration, float shadowAlpha = 0.5f, float shadowExistTime = 0.25f, float shadowSpawnInterval = 0.1f, Vector3 shadowOffset = default(Vector3))
    {
        float endTime = Time.time + duration;

        // Mảng các màu sắc để chọn ngẫu nhiên
        Color[] shadowColors = new Color[]
        {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta
        };

        while (Time.time < endTime)
        {
            GameObject shadow = Instantiate(shadowPrefab, transform.position + shadowOffset, transform.rotation);
            SpriteRenderer shadowSprite = shadow.GetComponent<SpriteRenderer>();
            shadowSprite.sprite = GetComponent<SpriteRenderer>().sprite;

            // Chọn ngẫu nhiên một màu từ mảng shadowColors
            Color shadowColor = shadowColors[Random.Range(0, shadowColors.Length)];
            shadowColor.a = shadowAlpha; // Độ trong suốt của bóng mờ
            shadowSprite.color = shadowColor;

            Destroy(shadow, shadowExistTime); // Bóng mờ tồn tại trong shadowExistTime giây

            yield return new WaitForSeconds(shadowSpawnInterval); // Tạo bóng mờ sau mỗi shadowSpawnInterval giây
        }
    }

    public void ApplyDebuffSpeed()
    {
        if (debuffCoroutine != null)
        {
            StopCoroutine(debuffCoroutine);
        }
        debuffCoroutine = StartCoroutine(DebuffSpeedCoroutine());
    }

    private IEnumerator DebuffSpeedCoroutine()
    {
        isDebuffed = true;
        StartCoroutine(CreateDebuffEffect(debuffDuration));
        float originalSpeed = moveSpeed;
        moveSpeed *= debuffPercentage; // Giảm tốc độ

        yield return new WaitForSeconds(debuffDuration);

        // Khôi phục tốc độ
        moveSpeed = originalSpeed;
        isDebuffed = false;

        debuffCoroutine = null;
    }

    private IEnumerator CreateDebuffEffect(float duration)
    {
        float endTime = Time.time + duration;
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        Color originalColor = playerSprite.color;

        while (Time.time < endTime)
        {
            playerSprite.color = Color.Lerp(originalColor, Color.gray, Mathf.PingPong(Time.time * 2, 1));
            yield return null;
        }

        playerSprite.color = originalColor;
    }
}