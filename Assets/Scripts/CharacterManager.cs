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

    private void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyAll()
    {
        Character[] characters = GetComponentsInChildren<Character>();
        foreach (Character c in characters)
        {
            Destroy(c.gameObject);
        }
    }
}
