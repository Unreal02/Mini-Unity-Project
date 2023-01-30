using UnityEngine;

public class GameManager : MonoBehaviour
{
    MapManager mapManager;
    Player player;

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        mapManager = MapManager.Instance;
        player = FindObjectOfType<Player>();

        mapManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mapManager.Init();
        }
    }
}
