using UnityEngine;

public class Bully : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask WhatIsObstacles;
    [SerializeField] private Vector2 movement;
    [SerializeField] private float Distance; // Khoảng cách hiện tại giữa Bully và người chơi
    [SerializeField] private float moveSpeed;
    [SerializeField] private float DistanceToPlayer; // Ngưỡng khoảng cách để Bully bắt đầu đi theo người chơi
    [SerializeField] private float MaxDistance; // Khoảng cách tối đa Bully có thể đi

    private Vector2 initialPosition;
    private Vector2 lastKnownPosition;
    private float timeSinceLastSeenPlayer = 0f;
    private bool playerInSight = false;
    private bool isMoving = false; // Biến kiểm soát trạng thái di chuyển của Bully
    private bool isPatrolling = false;
    private float patrolStartTime;
    [SerializeField] private float patrolDuration = 10f;
    [SerializeField] private float _patrolDistance;


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
        bool isPlayerHidden = IsPlayerHidden();
        Distance = Vector2.Distance(transform.position, player.transform.position);

        if (Distance < DistanceToPlayer && Distance <= MaxDistance && isPlayerHidden == false)
        {
            playerInSight = true;
            lastKnownPosition = player.transform.position;
            timeSinceLastSeenPlayer = 0f;
            isPatrolling = false;
        }
        else
        {
            timeSinceLastSeenPlayer += Time.deltaTime;

            if (timeSinceLastSeenPlayer > 10f)
            {
                playerInSight = false;
                isPatrolling = false;
            }
            else if (playerInSight)
            {
                playerInSight = false;
                isPatrolling = true;
                patrolStartTime = Time.time;
                timeSinceLastSeenPlayer = 0f;
            }
        }

        if (playerInSight)
        {
            MoveTowardsPlayer();
        }
        else if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            ReturnToInitialPosition();
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
        isMoving = true;
    }

    private void Patrol()
    {
        float patrolTimeElapsed = Time.time - patrolStartTime;
        if (patrolTimeElapsed < patrolDuration)
        {
            // Điều chỉnh khoảng cách tuần tra
            float patrolDistance = _patrolDistance; // Có thể thay đổi giá trị này để điều chỉnh khoảng cách tuần tra

            // Di chuyển qua lại giữa hai điểm xung quanh vị trí cuối cùng thấy người chơi
            Vector2 patrolPoint1 = lastKnownPosition + new Vector2(patrolDistance, 0);
            Vector2 patrolPoint2 = lastKnownPosition + new Vector2(-patrolDistance, 0);
            Vector2 targetPoint = (patrolTimeElapsed % 2 < 1) ? patrolPoint1 : patrolPoint2;

            if (Vector2.Distance(transform.position, targetPoint) > 0.1f)
            {
                movement = (targetPoint - rb2d.position).normalized;
                rb2d.MovePosition(Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime));
                animator.SetFloat("Horizontal", movement.x);
                animator.SetFloat("Vertical", movement.y);
                animator.SetFloat("Speed", movement.sqrMagnitude);
                isMoving = true;
            }
        }
        else
        {
            // Kết thúc tuần tra và quay lại vị trí ban đầu
            isPatrolling = false;
            ReturnToInitialPosition();
        }
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.01f)
        {
            movement = (initialPosition - rb2d.position).normalized;
            rb2d.MovePosition(Vector2.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime));
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            isMoving = true;
        }
        else
        {
            // Reset lại trạng thái khi Bully quay về vị trí ban đầu
            ResetBully();
        }
    }

    private bool IsPlayerHidden()
    {
        // Kiểm tra xem người chơi có nằm sau vật thể hay không bằng cách sử dụng Raycast
        RaycastHit2D hits = Physics2D.Linecast(transform.position, player.transform.position, WhatIsObstacles);

        // Nếu Raycast không va chạm với vật thể nào, tức là người chơi không bị che giấu
        return hits.collider != null;
    }

    private void ResetBully()
    {
        // Đảm bảo rằng Bully ngừng di chuyển hoàn toàn
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0f;

        // Reset lại các biến
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);
        animator.SetFloat("Speed", 0);
        animator.Play("BullyIdle"); // Chuyển sang trạng thái Idle bằng cách sử dụng tên animation
        playerInSight = false;
        isMoving = false;
        timeSinceLastSeenPlayer = 0f;
    }
}