using System.Collections;
using UnityEngine;

public class Bully : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 movement;
    [SerializeField] private float KhoangCach; // Khoảng cách hiện tại giữa Bully và người chơi
    [SerializeField] private float moveSpeed;
    [SerializeField] private float KhoangCachVoiNguoiChoi; // Ngưỡng khoảng cách để Bully bắt đầu theo dõi người chơi
    [SerializeField] private float MaxDistance; // Khoảng cách tối đa Bully có thể đi

    private Vector2 initialPosition;
    private float timeSinceLastSeenPlayer = 0f;
    private bool playerInSight = false;
    private bool isMoving = false; // Biến kiểm soát trạng thái di chuyển của Bully

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        KhoangCach = Vector2.Distance(transform.position, player.transform.position);

        if (KhoangCach < KhoangCachVoiNguoiChoi)
        {
            playerInSight = true;
            timeSinceLastSeenPlayer = 0f;
        }
        else
        {
            playerInSight = false;
            timeSinceLastSeenPlayer += Time.deltaTime;
        }

        if (timeSinceLastSeenPlayer > 10f)
        {
            ReturnToInitialPosition();
        }
        else if (playerInSight)
        {
            if (KhoangCach > MaxDistance)
            {
                playerInSight = false;
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb2d.MovePosition(rb2d.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void MoveTowardsPlayer()
    {
        movement = (Vector2)(player.transform.position - transform.position).normalized;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        isMoving = true;
    }

    private void ReturnToInitialPosition()
    {
        movement = (initialPosition - rb2d.position).normalized;
        rb2d.MovePosition(Vector2.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime));
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Kiểm tra khi Bully đã quay về vị trí ban đầu
        if (Vector2.Distance(transform.position, initialPosition) < 0.01f)
        {
            isMoving = false; // Tắt trạng thái di chuyển
        }
    }
}