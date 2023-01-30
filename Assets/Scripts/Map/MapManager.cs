using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private Map map;
    public TileBase[] innerTiles;
    public TileBase borderTile;
    public Tilemap tilemap;

    public Player player;

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

    void Awake()
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
        player = FindObjectOfType<Player>();
        map = new Map();

        PlaceTiles();
        PlacePlayer();
    }

    private void PlaceTiles()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < map.GetSize(); x++)
        {
            for (int y = 0; y < map.GetSize(); y++)
            {
                if (map.GetTile(x, y).type == MapTile.TileType.Ground)
                {
                    tilemap.SetTile(new Vector3Int(x, y), innerTiles[(x + y) % 2]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y), borderTile);
                }
            }
        }
    }

    private void PlacePlayer()
    {
        (int x, int y) = map.GetRandomPosition();
        player.MoveTo(x, y);
    }
}
