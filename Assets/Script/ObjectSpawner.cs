using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy }

    public Tilemap tilemap;
    public GameObject[] objectprefabs;
    public float bigGemProbability = 0.2f;
    public float enemyProbability = 0.1f;
    public int maxObjectsToSpawn = 5;
    public float gemLifetime = 5f;
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    void Update()
    {
        if (!isSpawning && ActiveObjectCount() < maxObjectsToSpawn)
        {
            isSpawning = true;
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    public void ResetSpawner()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }

        spawnedObjects.Clear();
        GatherValidPositions();
        StopAllCoroutines();
        isSpawning = false;
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    private int ActiveObjectCount()
    {
        spawnedObjects.RemoveAll(item => item == null);
        return spawnedObjects.Count;
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        while (ActiveObjectCount() < maxObjectsToSpawn)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnedObjects.Any(obj => obj != null && Vector3.Distance(obj.transform.position, positionToCheck) < 0.1f);
    }

    private ObjectType RandomObjectType()
    {
        float rand = Random.value;

        if (rand <= enemyProbability)
            return ObjectType.Enemy;
        else if (rand <= bigGemProbability + enemyProbability)
            return ObjectType.BigGem;
        else
            return ObjectType.SmallGem;
    }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0 || objectprefabs.Length == 0)
            return;

        int randomIndex = Random.Range(0, validSpawnPositions.Count);
        Vector3 spawnPosition = validSpawnPositions[randomIndex];

        // Skip if off camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(spawnPosition);
            if (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f)
            {
                validSpawnPositions.RemoveAt(randomIndex);
                return;
            }
        }

        // Skip if already occupied
        if (PositionHasObject(spawnPosition)) return;

        ObjectType objectType = RandomObjectType();
        GameObject prefabToSpawn = objectprefabs[(int)objectType];

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawned);

        // Destroy gems after a while
        if (objectType != ObjectType.Enemy)
        {
            Destroy(spawned, gemLifetime);
        }
    }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();

        if (tilemap == null)
        {
            Debug.LogWarning("[ObjectSpawner] Tilemap is not assigned.");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3 spawnPoint = start + new Vector3(x + 0.5f, y + 2f, 0);
                    validSpawnPositions.Add(spawnPoint);
                }
            }
        }

        if (validSpawnPositions.Count == 0)
        {
            Debug.LogWarning("[ObjectSpawner] No valid spawn positions found.");
        }
    }
}
