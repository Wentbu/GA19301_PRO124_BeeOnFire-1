using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static BullyController;

public class Node
{
    public Vector2 Position;
    public Node Parent;
    public float G, H, F;

    public Node(Vector2 position)
    {
        Position = position;
    }
}

public class AStar
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Node> openList;
    private List<Node> closedList;
    private float cellSize;
    private Vector2 gridSize;

    public AStar(float cellSize, Vector2 gridSize)
    {
        this.cellSize = cellSize;
        this.gridSize = gridSize;
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = new Node(startPos);
        Node targetNode = new Node(targetPos);

        openList = new List<Node> { startNode };
        closedList = new List<Node>();

        startNode.G = 0;
        startNode.H = CalculateDistanceCost(startNode, targetNode);
        startNode.F = startNode.G + startNode.H;

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            if (currentNode.Position == targetNode.Position)
            {
                return CalculatePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                float tentativeGCost = currentNode.G + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.G)
                {
                    neighbourNode.Parent = currentNode;
                    neighbourNode.G = tentativeGCost;
                    neighbourNode.H = CalculateDistanceCost(neighbourNode, targetNode);
                    neighbourNode.F = neighbourNode.G + neighbourNode.H;

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> GetNeighbourList(Node currentNode)
    {
        List<Node> neighbourList = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2 neighbourPos = new Vector2(
                    currentNode.Position.x + x * cellSize,
                    currentNode.Position.y + y * cellSize
                );

                if (neighbourPos.x >= 0 && neighbourPos.x < gridSize.x &&
                    neighbourPos.y >= 0 && neighbourPos.y < gridSize.y)
                {
                    neighbourList.Add(new Node(neighbourPos));
                }
            }
        }

        return neighbourList;
    }

    private List<Vector2> CalculatePath(Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private float CalculateDistanceCost(Node a, Node b)
    {
        float xDistance = Mathf.Abs(a.Position.x - b.Position.x);
        float yDistance = Mathf.Abs(a.Position.y - b.Position.y);
        float remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].F < lowestFCostNode.F)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }
}

public class BullyController : MonoBehaviour
{
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

    private AStar pathfinder;
    private List<Vector2> currentPath;
    private int currentPathIndex;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        initialPosition = transform.position;
        noiseOffsetX = Random.value * 1000f;
        noiseOffsetY = Random.value * 1000f;
        pathfinder = new AStar(1f, new Vector2(100, 100)); // Adjust grid size as needed
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

        MoveAlongPath();
    }

    private void MoveTowardsPlayer()
    {
        SetNewPath(player.position);
        isMoving = true;
    }

    private void Patrol()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count)
        {
            currentPatrolTarget = GenerateRandomPatrolPoint();
            SetNewPath(currentPatrolTarget);
        }
    }

    private void ReturnToInitialPosition()
    {
        if (Vector2.Distance(transform.position, initialPosition) > 0.01f)
        {
            SetNewPath(initialPosition);
            isMoving = true;
        }
        else
        {
            ResetBully();
        }
    }

    private void SetNewPath(Vector2 target)
    {
        currentPath = pathfinder.FindPath(transform.position, target);
        currentPathIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (currentPath != null && currentPathIndex < currentPath.Count)
        {
            Vector2 targetPosition = currentPath[currentPathIndex];
            Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb2d.MovePosition(rb2d.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }

            movement = moveDirection;
            UpdateAnimation();
        }
        else
        {
            movement = Vector2.zero;
            UpdateAnimation();
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