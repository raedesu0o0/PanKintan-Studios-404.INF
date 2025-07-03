using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType {SmallGem, BigGem, Enemy}
    public Tilemap tilemap;
    public GameObject [] objectprefabs; // Array of prefabs to spawn
    public float bigGemProbability = 0.2f; // Probability of spawning a big gem
    public float enemyProbability = 0.1f; // Probability of spawning an enemy
    public int maxObjectsToSpawn = 5; // Maximum number of objects to spawn
    public float gemLifetime = 5f; // Lifetime of the gems in seconds
    public float spawnInterval = 0.5f; // Time interval between spawns

    private List<Vector3Int> spawnPositions = new List<Vector3Int>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isSpawning = false;



    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawnObjectsIfNeeded());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int ActiveObjects
    {
        spawnObject.RemoveAll(item -> item = null)
        return spawnedObjects.Count; 
    }

    private IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawning = true;
        while (ActiveObjectCount() < maxObjects)
        {
            yiels return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
     }

    private void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while(!validPositionFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPosition = validSpawnPositions[randomIndex];
        }

        

        Vector3 spawnPosition = validSpawnPositions[Random.Range(0, validSpawnPositions.Count)];
        GameObject prefabToSpawn = objectprefabs[Random.Range(0, objectprefabs.Length)];

        // Determine the type of object to spawn based on probabilities
        float randomValue = Random.value;
        if (randomValue < enemyProbability)
        {
            prefabToSpawn = objectprefabs[2]; // Enemy prefab
        }
        else if (randomValue < bigGemProbability + enemyProbability)
        {
            prefabToSpawn = objectprefabs[1]; // Big gem prefab
        }
        else
        {
            prefabToSpawn = objectprefabs[0]; // Small gem prefab
        }

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(spawnedObject);

        // Set lifetime for gems
        if (spawnedObject.CompareTag("Gem"))
        {
            Destroy(spawnedObject, gemLifetime);
        }
    }

    {

    }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; y < bounds.size.x; x++)
        {
            for (int y = 0; x < bounds.size.y; y++)
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
