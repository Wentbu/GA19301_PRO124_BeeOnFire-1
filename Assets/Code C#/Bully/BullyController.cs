using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class BullyController : MonoBehaviour
{
    private Seeker seeker;
    private AIPath aiPath;

    [Header("Interface")]
    [SerializeField] Animator animator;
    [SerializeField] private Vector2 movement;
    [SerializeField] Rigidbody2D rb2d;

    [Header("Obstacles")]
    [SerializeField] private LayerMask WhatIsObstacles;
    [SerializeField] private float avoidanceRadius = 1f;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Behaviour")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private float patrolDuration;
    [SerializeField] private float patrolRadius;
    [SerializeField] private float distance;
    [SerializeField] private float distanceToPlayer; // Ngưỡng khoảng cách để Bully bắt đầu đi theo người chơi
    [SerializeField] private float maxDistance; // Khoảng cách tối đa Bully có thể đi
    [SerializeField] private float maxSearchTime = 10f;
    [SerializeField] private float maxChaseTime = 5f;
    [SerializeField] private float randomnessFactor = 0.2f;
    [SerializeField] private float predictionTime = 1f;

    private Vector2[] patrolPoints;
    private float[,] transitionMatrix;
    private int currentPatrolIndex = 0;
    private Vector2 initialPosition;
    private Vector2 lastKnownPlayerPosition;
    private float searchTime = 0f;
    private List<Vector2> searchPoints = new List<Vector2>();
    private int currentSearchIndex = 0;
    private float timeSinceLastSearch = 0f;
    private float timeSinceLastSeenPlayer = 0f;
    private float patrolTimer = 0f;

    private enum BullyState
    {
        Idle,
        Chasing,
        Searching,
        Patrolling,
        ReturningToStart
    }

    [SerializeField] private BullyState currentState = BullyState.Idle;

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
        InitializePatrolSystem();
    }

    void Update()
    {
        UpdateState();
        ExecuteCurrentState();
        distance = Vector2.Distance(transform.position, player.transform.position);
        movement = AvoidObstacles(movement);
        rb2d.velocity = movement * moveSpeed;
        UpdateAnimation();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case BullyState.Idle:
                if (distance < distanceToPlayer && distance <= maxDistance && CanSeePlayer())
                {
                    currentState = BullyState.Chasing;
                }
                break;

            case BullyState.Chasing:
                if (!CanSeePlayer())
                {
                    if (timeSinceLastSeenPlayer > maxChaseTime)
                    {
                        currentState = BullyState.Searching;
                        searchTime = 0f;
                    }
                }
                break;

            case BullyState.Searching:
                searchTime += Time.deltaTime;
                if (distance < distanceToPlayer && distance <= maxDistance && CanSeePlayer())
                {
                    currentState = BullyState.Chasing;
                    searchTime = 0f;
                }
                else if (searchTime >= maxSearchTime)
                {
                    currentState = BullyState.Patrolling;
                    patrolTimer = 0f;
                }
                break;

            case BullyState.Patrolling:
                patrolTimer += Time.deltaTime;
                if (distance < distanceToPlayer && distance <= maxDistance && CanSeePlayer())
                {
                    currentState = BullyState.Chasing;
                    patrolTimer = 0f;
                }
                else if (patrolTimer >= patrolDuration)
                {
                    currentState = BullyState.ReturningToStart;
                    patrolTimer = 0f;
                }
                break;

            case BullyState.ReturningToStart:
                if (distance < distanceToPlayer && distance <= maxDistance && CanSeePlayer())
                {
                    currentState = BullyState.Chasing;
                    patrolTimer = 0f;
                }
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case BullyState.Idle:
                ResetBully();
                break;
            case BullyState.Chasing:
                MoveTowardsPredictedPosition();
                break;
            case BullyState.Patrolling:
                Patrol();
                break;
            case BullyState.Searching:
                SearchForPlayer();
                break;
            case BullyState.ReturningToStart:
                ReturnToInitialPosition();
                break;
        }
    }

    private bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Kiểm tra nếu người chơi ở trong khoảng cách hợp lệ
        if (distanceToPlayer < distance || distanceToPlayer > maxDistance)
        {
            timeSinceLastSeenPlayer += Time.deltaTime;
            return false;
        }

        // Kiểm tra có vật cản giữa NPC và người chơi
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, WhatIsObstacles);
        if (hit.collider == null || hit.collider.CompareTag("Player"))
        {
            lastKnownPlayerPosition = player.position;
            timeSinceLastSeenPlayer = 0f;
            return true;
        }

        timeSinceLastSeenPlayer += Time.deltaTime;
        return false;
    }


    private void MoveTowardsPredictedPosition()
    {
        Vector2 predictedPosition = PredictPlayerPosition();
        seeker.StartPath(transform.position, predictedPosition, OnPathComplete);
        aiPath.maxSpeed = moveSpeed * (1 + Random.Range(-0.1f, 0.1f));
        movement = aiPath.desiredVelocity;
    }

    private Vector2 PredictPlayerPosition()
    {
        Vector2 playerVelocity = (Vector2)player.position - lastKnownPlayerPosition;
        Vector2 predictedPosition = (Vector2)player.position + playerVelocity * predictionTime;

        float maxPredictionDistance = maxDistance;
        if (Vector2.Distance(transform.position, predictedPosition) > maxPredictionDistance)
        {
            predictedPosition = (Vector2)transform.position + (predictedPosition - (Vector2)transform.position).normalized * maxPredictionDistance;
        }

        return predictedPosition;
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
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (aiPath.reachedEndOfPath)
        {
            int nextPatrolIndex = ChooseNextPatrolPoint();
            Vector2 nextPatrolPoint = patrolPoints[nextPatrolIndex];
            seeker.StartPath(transform.position, nextPatrolPoint, OnPathComplete);
            currentPatrolIndex = nextPatrolIndex;
        }

        movement = aiPath.desiredVelocity;
        UpdateAnimation();
    }

    private int ChooseNextPatrolPoint()
    {
        float randomValue = Random.value;
        if (randomValue < randomnessFactor)
        {
            return Random.Range(0, patrolPoints.Length);
        }
        else
        {
            float[] probabilities = new float[patrolPoints.Length];
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                probabilities[i] = transitionMatrix[currentPatrolIndex, i];
            }

            float total = probabilities.Sum();
            float randomPoint = Random.value * total;

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (randomPoint < probabilities[i])
                {
                    return i;
                }
                randomPoint -= probabilities[i];
            }
            return (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ReturnToInitialPosition()
    {
        Vector2 currentPosition = transform.position;
        float distanceToInitialPosition = Vector2.Distance(currentPosition, initialPosition);

        // Kiểm tra nếu NPC đã gần đến vị trí ban đầu
        if (distanceToInitialPosition > 0.1f)
        {
            // Di chuyển NPC về vị trí ban đầu
            Vector2 direction = (initialPosition - currentPosition).normalized;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, initialPosition, moveSpeed * Time.deltaTime);
            transform.position = newPosition;

            // Cập nhật animation di chuyển
            movement = direction;
            UpdateAnimation();
        }
        else
        {
            ResetBully();
        }
    }

    private void SearchForPlayer()
    {
        searchTime += Time.deltaTime;
        timeSinceLastSearch += Time.deltaTime;

        if (searchTime <= maxSearchTime)
        {
            // Tạo các điểm tìm kiếm nếu chưa có
            if (searchPoints.Count == 0)
            {
                GenerateSearchPoints();
            }

            // Tìm kiếm theo khoảng thời gian
            if (timeSinceLastSearch >= 1f) // Tìm kiếm mỗi giây
            {
                Vector2 nextSearchPoint = GetNextSearchPoint();
                seeker.StartPath(transform.position, nextSearchPoint, OnPathComplete);
                movement = aiPath.desiredVelocity;
                timeSinceLastSearch = 0f;
            }
        }
        else
        {
            // Chuyển đổi trạng thái mượt mà
            StartCoroutine(TransitionToPatrolling());
        }
    }

    private void GenerateSearchPoints()
    {
        // Tạo điểm tìm kiếm theo mô hình xoắn ốc
        int numPoints = 10;
        float angleStep = 2f * Mathf.PI / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            float angle = i * angleStep;
            float radius = Mathf.Lerp(0, searchRadius, (float)i / numPoints);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            searchPoints.Add(lastKnownPlayerPosition + offset);
        }

        // Thêm một số điểm ngẫu nhiên
        for (int i = 0; i < 5; i++)
        {
            searchPoints.Add(lastKnownPlayerPosition + Random.insideUnitCircle * searchRadius);
        }

        // Sắp xếp các điểm theo khoảng cách tăng dần
        searchPoints.Sort((a, b) => Vector2.Distance(transform.position, a).CompareTo(Vector2.Distance(transform.position, b)));
    }

    private Vector2 GetNextSearchPoint()
    {
        Vector2 nextPoint = searchPoints[currentSearchIndex];
        currentSearchIndex = (currentSearchIndex + 1) % searchPoints.Count;
        return nextPoint;
    }

    private IEnumerator TransitionToPatrolling()
    {
        float transitionDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            // Giảm dần tốc độ
            aiPath.maxSpeed = Mathf.Lerp(moveSpeed, 0, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = BullyState.Patrolling;
        aiPath.maxSpeed = moveSpeed; // Reset tốc độ
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

    private void InitializePatrolSystem()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            patrolPoints = new Vector2[]
            {
                initialPosition + Vector2.right * 5,
                initialPosition + Vector2.up * 5,
                initialPosition + Vector2.left * 5,
                initialPosition + Vector2.down * 5
            };
        }

        int pointCount = patrolPoints.Length;
        transitionMatrix = new float[pointCount, pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            for (int j = 0; j < pointCount; j++)
            {
                if (i == j) continue;
                float distance = Vector2.Distance(patrolPoints[i], patrolPoints[j]);
                transitionMatrix[i, j] = 1f / distance;
            }

            float sum = 0;
            for (int j = 0; j < pointCount; j++)
            {
                sum += transitionMatrix[i, j];
            }
            for (int j = 0; j < pointCount; j++)
            {
                transitionMatrix[i, j] /= sum;
            }
        }
    }

    private void ResetBully()
    {
        // Đảm bảo rằng Bully ngừng di chuyển hoàn toàn
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0f;
        rb2d.Sleep(); // Dừng tất cả các hoạt động vật lý trên Rigidbody2D

        // Đặt lại các tham số của animator
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Speed", 0f);
        animator.Play("BullyIdle");

        // Đặt lại các trạng thái hoặc biến khác của NPC
        currentState = BullyState.Idle; // Đặt lại trạng thái về Idle
        movement = Vector2.zero; // Đặt lại biến chuyển động
        timeSinceLastSearch = 0f; //Đặt lại thời gian tìm kiếm
        timeSinceLastSeenPlayer = 0f; // Đặt lại biến theo dõi thời gian
        searchTime = 0f; // Đặt lại thời gian tìm kiếm
        patrolTimer = 0f; // Đặt lại thời gian tuần tra
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }
}