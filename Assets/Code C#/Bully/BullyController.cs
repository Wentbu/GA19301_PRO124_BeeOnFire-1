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
    [SerializeField] private float waitTime = 1f;
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

            patrolTimer = 0f; // Reset bộ đếm thời gian khi thấy người chơi
        }
        else if (isPatrolling)
        {
            StartCoroutine(WaitBeforePatrol());
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
        animator.Play("BullyIdle"); // Chuyển sang trạng thái Idle bằng cách sử dụng tên animation
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
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        Vector2 randomPoint = initialPosition + randomDirection;
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
                // Tìm điểm xa nhất có thể đi được
                Vector2 furthestReachablePoint = path.corners[path.corners.Length - 1];

                // Tìm điểm gần nhất trên NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(furthestReachablePoint, out hit, searchRadius, NavMesh.AllAreas))
                {
                    // Tìm đường đi mới từ điểm hiện tại đến điểm gần nhất trên NavMesh
                    NavMeshPath newPath = new NavMeshPath();
                    if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, newPath))
                    {
                        agent.SetPath(newPath);
                    }
                }
            }
            else
            {
                agent.SetPath(path);
            }
        }
    }

    IEnumerator WaitBeforePatrol()
    {
        yield return new WaitForSeconds(waitTime);
    }
}