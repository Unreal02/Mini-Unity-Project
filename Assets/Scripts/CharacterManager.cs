using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    static CharacterManager instance;
    public static CharacterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CharacterManager>();
            }
            return instance;
        }
    }

    static Player player;
    public static Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();
            }
            return player;
        }
    }

    private void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyPlayer()
    {
        Player[] players = GetComponentsInChildren<Player>();
        foreach (Player p in players)
        {
            Destroy(p.gameObject);
        }
    }

    public void DestroyEnemies()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>();
        foreach (Enemy e in enemies)
        {
            Destroy(e.gameObject);
        }
    }

    public bool IsWaitFinished()
    {
        Character[] characters = GetComponentsInChildren<Character>();
        foreach (Character c in characters)
        {
            if (!c.IsIdleOrWaiting()) { return false; }
        }
        return true;
    }
}
