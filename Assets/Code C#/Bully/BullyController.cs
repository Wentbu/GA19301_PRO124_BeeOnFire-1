using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static BullyController;

public class BullyController : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] Animator animator;
    [SerializeField] private Vector2 movement;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private LayerMask WhatIsObstacles;
    [SerializeField] private float Distance; // Khoảng cách hiện tại giữa Bully và người chơi
    [SerializeField] private float DistanceToPlayer; // Ngưỡng khoảng cách để Bully bắt đầu đi theo người chơi
    [SerializeField] private float MaxDistance; // Khoảng cách tối đa Bully có thể đi

    [SerializeField] private Transform player;
    private Vector2 initialPosition;
    private Vector2 lastKnownPosition;
    private float timeSinceLastSeenPlayer = 0f;
    private bool playerInSight = false;
    private bool isMoving = false; // Biến kiểm soát trạng thái di chuyển của Bully
    private bool isPatrolling = false;
    private float patrolStartTime;
    private Vector2 currentPatrolTarget;
    private float noiseOffsetX;
    private float noiseOffsetY;
    [SerializeField] private float searchRadius = 10f;
    private float patrolTimer = 0f;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float patrolRadius;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        initialPosition = transform.position;
        noiseOffsetX = Random.value * 1000f;
        noiseOffsetY = Random.value * 1000f;
    }

    // Update is called once per frame
    void Update()
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

            if (timeSinceLastSeenPlayer > patrolDuration)
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
            patrolTimer = 0f;
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

    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.position);
        FindBetterPath(player.position);
        movement = agent.velocity;
        UpdateAnimation();
        isMoving = true;
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolTarget = GenerateRandomPatrolPoint();
            agent.SetDestination(currentPatrolTarget);
        }
        movement = agent.velocity;
        UpdateAnimation();
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.01f)
        {
            agent.SetDestination(initialPosition);
            FindBetterPath(player.position);
            movement = agent.velocity;
            UpdateAnimation();
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
        animator.Play("BullyIdle");
        playerInSight = false;
        isMoving = false;
        timeSinceLastSeenPlayer = 0f;
    }

    private void UpdateAnimation()
    {
        movement = agent.velocity;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private Vector2 GenerateRandomPatrolPoint()
    {
        float noiseX = Mathf.PerlinNoise(noiseOffsetX, Time.time * 0.1f) * 2 - 1;
        float noiseY = Mathf.PerlinNoise(noiseOffsetY, Time.time * 0.1f) * 2 - 1;
        Vector2 noiseDirection = new Vector2(noiseX, noiseY).normalized;

        Vector2 randomPoint = initialPosition + noiseDirection * patrolRadius;
        Vector3 randomPoint3D = new Vector3(randomPoint.x, randomPoint.y, 0);
        if (NavMesh.SamplePosition(randomPoint3D, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            return new Vector2(hit.position.x, hit.position.y);
        }
        return new Vector2(transform.position.x, transform.position.y);
    }

    private void FindBetterPath(Vector2 target)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathPartial)
            {
                Vector2 furthestReachablePoint = path.corners[path.corners.Length - 1];
                Vector2 direction = (furthestReachablePoint - (Vector2)transform.position).normalized;

                // Sử dụng kỹ thuật ray casting để tìm điểm xa nhất có thể đi được
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, searchRadius, WhatIsObstacles);
                if (hit.collider != null)
                {
                    Vector2 newTarget = hit.point - direction * 0.1f; // Lùi lại một chút để tránh va chạm
                    agent.SetDestination(newTarget);
                }
                else
                {
                    agent.SetDestination(furthestReachablePoint);
                }
            }
            else
            {
                agent.SetPath(path);
            }
        }
    }
}