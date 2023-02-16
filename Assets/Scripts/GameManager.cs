using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    MapManager mapManager;
    CharacterManager characterManager;

    public UnityEvent playerActionEvent = new UnityEvent();
    public UnityEvent stageClearEvent = new UnityEvent();
    public UnityEvent playerDeathEvent = new UnityEvent();

    private int _currentStage;
    private int currentStage
    {
        get => _currentStage;
        set
        {
            _currentStage = value;
            GameObject.Find("Current Stage Text").GetComponent<TextMeshProUGUI>().text = string.Format("Stage {0}", _currentStage);
        }
    }

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
        foreach (ObjectPool pool in FindObjectsOfType<ObjectPool>())
        {
            pool.Init();
        }

        mapManager.Init();

        currentStage = 1;
        stageClearEvent.AddListener(() => { SetNextStage(); });
        playerDeathEvent.AddListener(ResetGame);
    }

    // Update is called once per frame
    private void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGame();
        }
#endif
    }

    private void SetNextStage()
    {
        _ = StartCoroutine(nameof(_SetNextStage));
    }

    private IEnumerator _SetNextStage()
    {
        ScreenPanel.Instance.Fade();
        yield return new WaitForSeconds(ScreenPanel.fadeTime);
        currentStage++;
        characterManager.DestroyEnemies();
        mapManager.Init();
    }

    private void ResetGame()
    {
        _ = StartCoroutine(nameof(_ResetGame));
    }

    private IEnumerator _ResetGame()
    {
        ScreenPanel.Instance.Fade();
        yield return new WaitForSeconds(ScreenPanel.fadeTime);
        currentStage = 1;
        characterManager.DestroyPlayer();
        characterManager.DestroyEnemies();
        mapManager.Init();
    }
}
