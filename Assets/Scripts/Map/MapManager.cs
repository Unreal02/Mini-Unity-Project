using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Map map;
    public TileBase[] innerTiles;
    public TileBase borderTile;
    public TileBase goalTile;
    public Tilemap tilemap;

    public GameObject player;
    public GameObject enemy;

    const int enemyNumber = 10;

    static MapManager instance;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        if (innerTiles.Length != 2)
        {
            Debug.LogErrorFormat("innerTiles.Length should be 2, but it is {0}", innerTiles.Length);
        }
    }

    public void Init()
    {
        map = new Map();

        PlaceTiles();
        SpawnPlayer();
        SpawnEnemies();
    }

    private void PlaceTiles()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < map.GetSize(); x++)
        {
            for (int y = 0; y < map.GetSize(); y++)
            {
                switch (map.GetTile(x, y).type)
                {
                    case MapTile.TileType.Ground:
                        tilemap.SetTile(new Vector3Int(x, y), innerTiles[(x + y) % 2]);
                        break;
                    case MapTile.TileType.Wall:
                        tilemap.SetTile(new Vector3Int(x, y), borderTile);
                        break;
                    case MapTile.TileType.Goal:
                        tilemap.SetTile(new Vector3Int(x, y), goalTile);
                        break;
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        int x, y;
        do
        {
            (x, y) = map.GetRandomPosition();
        } while (!map.GetTile(x, y).IsMovable());
        Player playerComponent = FindObjectOfType<Player>();
        if (playerComponent == null)
        {
            playerComponent = Instantiate(player, new Vector3(x, y), Quaternion.identity, CharacterManager.Instance.transform).GetComponent<Player>();
            playerComponent.Init();
        }
        else
        {
            playerComponent.MoveTo(x, y);
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyNumber; i++)
        {
            int x, y;
            do
            {
                (x, y) = map.GetRandomPosition();
            } while (!map.GetTile(x, y).IsMovable());
            Enemy EnemyComponent = Instantiate(enemy, new Vector3(x, y), Quaternion.identity, CharacterManager.Instance.transform).GetComponent<Enemy>();
            EnemyComponent.Init();
        }
    }
}
