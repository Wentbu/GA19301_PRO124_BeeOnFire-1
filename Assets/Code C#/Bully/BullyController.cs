using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using static BullyController;

public class BullyController : MonoBehaviour
{
    private Seeker seeker;
    private AIPath aiPath;

    [SerializeField] Animator animator;
    [SerializeField] private Vector2 movement;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] private float avoidanceRadius = 1f;
    [SerializeField] private LayerMask WhatIsObstacles;
    [SerializeField] private float Distance;
    [SerializeField] private float DistanceToPlayer;
    [SerializeField] private float MaxDistance;
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float viewDistance = 10f;
    [SerializeField] private float maxSearchTime = 10f;
    [SerializeField] private Vector2[] patrolPoints;
    [SerializeField] private float predictionTime = 1f;

    private int currentPatrolIndex = 0;
    private Vector2 initialPosition;
    private Vector2 lastKnownPlayerPosition;
    private bool isSearching = false;
    private float searchTime = 0f;
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
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            playerInSight = true;
            lastKnownPlayerPosition = player.position;
            timeSinceLastSeenPlayer = 0f;
            isPatrolling = false;
            isSearching = false;
            MoveTowardsPredictedPosition();
        }
        else
        {
            timeSinceLastSeenPlayer += Time.deltaTime;

            if (timeSinceLastSeenPlayer > patrolDuration)
            {
                playerInSight = false;
                isPatrolling = true;
                isSearching = false;
            }
            else if (playerInSight)
            {
                SearchForPlayer();
            }
        }

        if (isPatrolling)
        {
            Patrol();
        }
        else if (!playerInSight && !isSearching)
        {
            ReturnToInitialPosition();
        }

        movement = AvoidObstacles(movement);

        rb2d.velocity = movement * moveSpeed;
        UpdateAnimation();
    }

    private bool CanSeePlayer()
    {
        if (Vector2.Distance(transform.position, player.position) > viewDistance)
            return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.Angle(transform.right, directionToPlayer);

        if (angle > fieldOfView / 2f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, WhatIsObstacles);
        return hit.collider == null || hit.collider.CompareTag("Player");
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
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            Vector2 nextPatrolPoint = patrolPoints[currentPatrolIndex];
            seeker.StartPath(transform.position, nextPatrolPoint, OnPathComplete);
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

    private void SearchForPlayer()
    {
        if (!isSearching)
        {
            isSearching = true;
            searchTime = 0f;
            StartCoroutine(SearchCoroutine());
        }
    }

    private IEnumerator SearchCoroutine()
    {
        while (searchTime < maxSearchTime)
        {
            Vector2 searchPoint = lastKnownPlayerPosition + Random.insideUnitCircle * 5f;
            seeker.StartPath(transform.position, searchPoint, OnPathComplete);
            yield return new WaitForSeconds(2f);
            searchTime += 2f;

            if (CanSeePlayer())
            {
                isSearching = false;
                yield break;
            }
        }
        isSearching = false;
    }

    private Vector2 PredictPlayerPosition()
    {
        Vector2 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
        return (Vector2)player.position + playerVelocity * predictionTime;
    }

    private void MoveTowardsPredictedPosition()
    {
        Vector2 predictedPosition = PredictPlayerPosition();
        seeker.StartPath(transform.position, predictedPosition, OnPathComplete);
        movement = aiPath.desiredVelocity;
        UpdateAnimation();
        isMoving = true;
    }

    private Vector2 AvoidObstacles(Vector2 currentMovement)
    {
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, WhatIsObstacles);
        Vector2 avoidanceForce = Vector2.zero;

        foreach (Collider2D obstacle in obstacles)
        {
            Vector2 directionToObstacle = obstacle.transform.position - transform.position;
            float distance = directionToObstacle.magnitude;
            avoidanceForce += -directionToObstacle.normalized * (avoidanceRadius / distance);
        }

        return (currentMovement + avoidanceForce).normalized;
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

        GraphNode node = AstarPath.active.GetNearest(randomPoint).node;
        if (node != null && node.Walkable)
        {
            return (Vector3)node.position;
        }
        return transform.position;
    }
}