using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType {SmallGem, BigGem, Enemy}

    public Tilemap.tilemap;
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
    }

    // Update is called once per frame
    void Update()
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
