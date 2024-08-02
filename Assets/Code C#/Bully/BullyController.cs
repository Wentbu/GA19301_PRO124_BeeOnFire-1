using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using static BullyController;

public class BullyController : MonoBehaviour
{
    private Seeker seeker;
    private AIPath aiPath;
    private NavMeshAgent agent;

    [SerializeField] Animator animator;
    [SerializeField] private Vector2 movement;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private LayerMask WhatIsObstacles;
    [SerializeField] private float Distance;
    [SerializeField] private float DistanceToPlayer;
    [SerializeField] private float MaxDistance;
    [SerializeField] private Transform player;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 initialPosition;
    private Vector2 lastKnownPosition;
    private float timeSinceLastSeenPlayer = 0f;
    private bool playerInSight = false;
    private bool isMoving = false;
    private bool isPatrolling = false;
    private float patrolStartTime;
    private Vector2 currentPatrolTarget;
    private float noiseOffsetX;
    private float noiseOffsetY;
    private float patrolTimer = 0f;

    private List<Vector2> currentPath;
    private int currentPathIndex;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        aiPath.enableRotation = false;
        initialPosition = transform.position;
        noiseOffsetX = Random.value * 1000f;
        noiseOffsetY = Random.value * 1000f;
    }

    void Update()
    {
        bool isPlayerHidden = IsPlayerHidden();
        Distance = Vector2.Distance(transform.position, player.transform.position);

        if (Distance < DistanceToPlayer && Distance <= MaxDistance && !isPlayerHidden)
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

        // Cập nhật movement dựa trên aiPath
        movement = aiPath.desiredVelocity;
        UpdateAnimation();
    }

    private void MoveTowardsPlayer()
    {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
        movement = aiPath.desiredVelocity;
        UpdateAnimation();
        isMoving = true;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            aiPath.canMove = true;
        }
    }

    private void Patrol()
    {
        if (aiPath.reachedEndOfPath)
        {
            currentPatrolTarget = GenerateRandomPatrolPoint();
            seeker.StartPath(transform.position, currentPatrolTarget, OnPathComplete);
        }
        movement = aiPath.desiredVelocity;
        UpdateAnimation();
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.01f)
        {
            seeker.StartPath(transform.position, initialPosition, OnPathComplete);
            movement = aiPath.desiredVelocity;
            UpdateAnimation();
            isMoving = true;
        }
        else
        {
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

        // Sử dụng AstarPath.active.GetNearest() thay vì NavMesh.SamplePosition
        GraphNode node = AstarPath.active.GetNearest(randomPoint).node;
        if (node != null && node.Walkable)
        {
            return (Vector3)node.position;
        }
        return transform.position;
    }
}