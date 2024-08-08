using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System;

[System.Serializable]
public class ItemType
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public float spawnChance; // Tỷ lệ xuất hiện (0-1)
}

public class SpawnItemss : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] public List<ItemType> itemTypes = new List<ItemType>();
    [SerializeField] public int numberOfItemsToSpawn = 10;
    [SerializeField] public float minDistanceBetweenItems = 1f;
    [SerializeField] public float itemOffsetFromGround = 0.1f;

    [Header("Map Settings")]
    [SerializeField] public Tilemap groundTilemap; // Kéo và thả Tilemap vào đây trong Inspector
    [SerializeField] public LayerMask[] obstacleLayers; // Mảng các LayerMask cho các lớp chướng ngại vật

    private ItemType[] cumulativeChances;
    private List<Vector3Int> validRoadTiles = new List<Vector3Int>();
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

    void Start()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("Road Tilemap is not assigned!");
            return;
        }

        NormalizeSpawnChances();
        PrepareItemChances();
        CacheValidRoadTiles();
        SpawnItems();
    }

    void NormalizeSpawnChances()
    {
        float totalChance = 0f;
        foreach (var item in itemTypes)
        {
            totalChance += item.spawnChance;
        }
        if (Mathf.Abs(totalChance - 1f) > 0.001f)
        {
            for (int i = 0; i < itemTypes.Count; i++)
            {
                itemTypes[i].spawnChance /= totalChance;
            }
        }
    }

    void PrepareItemChances()
    {
        cumulativeChances = new ItemType[itemTypes.Count];
        float cumulativeChance = 0f;
        for (int i = 0; i < itemTypes.Count; i++)
        {
            cumulativeChance += itemTypes[i].spawnChance;
            cumulativeChances[i] = new ItemType { prefab = itemTypes[i].prefab, spawnChance = cumulativeChance };
        }
    }

    void CacheValidRoadTiles()
    {
        BoundsInt bounds = groundTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (groundTilemap.HasTile(cellPosition))
                {
                    validRoadTiles.Add(cellPosition);
                }
            }
        }
    }

    void SpawnItems()
    {
        int positionsToGenerate = numberOfItemsToSpawn * 2; // Generate more positions than needed
        NativeArray<float2> positions = new NativeArray<float2>(positionsToGenerate, Allocator.TempJob);
        NativeArray<bool> validPositions = new NativeArray<bool>(positionsToGenerate, Allocator.TempJob);

        for (int i = 0; i < positionsToGenerate; i++)
        {
            positions[i] = GetRandomPointOnRoad2D();
        }

        SpawnJob job = new SpawnJob
        {
            potentialPositions = positions,
            minDistanceSqr = minDistanceBetweenItems * minDistanceBetweenItems,
            isValidPosition = validPositions
        };

        JobHandle jobHandle = job.Schedule(positionsToGenerate, 64);
        jobHandle.Complete();

        int spawnedCount = 0;
        for (int i = 0; i < positionsToGenerate && spawnedCount < numberOfItemsToSpawn; i++)
        {
            if (validPositions[i] && IsValidSpawnPoint(positions[i]))
            {
                Vector2 spawnPosition = positions[i];
                spawnPosition.y += itemOffsetFromGround;
                GameObject itemToSpawn = ChooseRandomItem();
                Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
                spawnedCount++;
            }
        }

        positions.Dispose();
        validPositions.Dispose();
    }

    GameObject ChooseRandomItem()
    {
        float randomValue = UnityEngine.Random.value;
        int index = Array.BinarySearch(cumulativeChances, new ItemType { spawnChance = randomValue },
            Comparer<ItemType>.Create((a, b) => a.spawnChance.CompareTo(b.spawnChance)));
        if (index < 0) index = ~index;
        return cumulativeChances[index].prefab;
    }

    Vector2 GetRandomPointOnRoad2D()
    {
        if (validRoadTiles.Count == 0) return Vector2.zero;
        Vector3Int randomCell = validRoadTiles[UnityEngine.Random.Range(0, validRoadTiles.Count)];
        return groundTilemap.GetCellCenterWorld(randomCell);
    }

    bool IsValidSpawnPoint(Vector2 point)
    {
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(point.x / minDistanceBetweenItems),
                                         Mathf.RoundToInt(point.y / minDistanceBetweenItems));

        // Kiểm tra xem ô và các ô lân cận đã bị chiếm chưa
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (occupiedCells.Contains(cell + new Vector2Int(dx, dy)))
                    return false;
            }
        }

        // Kiểm tra va chạm với chướng ngại vật
        foreach (var layer in obstacleLayers)
        {
            if (Physics2D.OverlapCircle(point, 0.1f, layer))
                return false;
        }

        occupiedCells.Add(cell);
        return true;
    }

    // Phương thức này có thể được gọi từ bên ngoài để spawn thêm items trong game
    public void SpawnAdditionalItems(int count)
    {
        numberOfItemsToSpawn = count;
        SpawnItems();
    }
}

public struct SpawnJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float2> potentialPositions;
    [ReadOnly] public float minDistanceSqr;
    public NativeArray<bool> isValidPosition;

    public void Execute(int index)
    {
        float2 currentPos = potentialPositions[index];
        for (int i = 0; i < index; i++)
        {
            if (math.distancesq(currentPos, potentialPositions[i]) < minDistanceSqr)
            {
                isValidPosition[index] = false;
                return;
            }
        }
        isValidPosition[index] = true;
    }
}