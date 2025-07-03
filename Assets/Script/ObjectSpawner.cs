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
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    public void ResetSpawner()
    {
        // Destroy all spawned objects
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();

        // Re-gather valid positions
        GatherValidPositions();

        // Start spawning again
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
        isSpawning = true;
        while (ActiveObjectCount() < maxObjectsToSpawn)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnedObjects.Any(checkObj => checkObj != null && Vector3.Distance(checkObj.transform.position, positionToCheck) < 0.1f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;
        if (randomChoice <= enemyProbability)
            return ObjectType.Enemy;
        else if (randomChoice <= bigGemProbability + enemyProbability)
            return ObjectType.BigGem;
        else
            return ObjectType.SmallGem;
    }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        int randomIndex = Random.Range(0, validSpawnPositions.Count);
        Vector3 spawnPosition = validSpawnPositions[randomIndex];

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(spawnPosition);
            if (viewPos.x < 0f || viewPos.x > 1f || viewPos.y < 0f || viewPos.y > 1f)
            {
                validSpawnPositions.RemoveAt(randomIndex);
                return;
            }
        }

        ObjectType objectType = RandomObjectType();
        GameObject prefabToSpawn = objectprefabs[(int)objectType];

        Vector3 leftPosition = spawnPosition + Vector3.left;
        Vector3 rightPosition = spawnPosition + Vector3.right;
        if (PositionHasObject(leftPosition) || PositionHasObject(rightPosition))
        {
            validSpawnPositions.RemoveAt(randomIndex);
            return;
        }

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawnedObject);

        if (objectType != ObjectType.Enemy)
        {
            Destroy(spawnedObject, gemLifetime);
        }
    }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
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
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }
}
