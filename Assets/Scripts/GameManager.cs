using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    MapManager mapManager;
    CharacterManager characterManager;

    public UnityEvent playerActionEvent = new UnityEvent();

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

    private void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        mapManager = MapManager.Instance;
        characterManager = CharacterManager.Instance;

        mapManager.Init();
    }

    // Update is called once per frame
    private void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            characterManager.DestroyAll();
            mapManager.Init();
        }
#endif
    }
}
