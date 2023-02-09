using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static ObjectPool instance;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool>();
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

    public GameObject hpBarPrefab;
    const int poolSize = 10;

    private Queue<HpBar> availableHpBar;

    public void Init()
    {
        availableHpBar = new Queue<HpBar>();
        SpawnPool();
    }

    private void SpawnPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            HpBar hpBar = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
            hpBar.gameObject.SetActive(false);
            availableHpBar.Enqueue(hpBar);
        }
    }

    public HpBar GetHpBar(Character character)
    {
        if (availableHpBar.Count == 0)
        {
            SpawnPool();
        }
        HpBar hpBar = availableHpBar.Dequeue();
        hpBar.gameObject.SetActive(true);
        hpBar.Init(character);
        return hpBar;
    }

    public void ReturnHpBar(HpBar hpBar)
    {
        availableHpBar.Enqueue(hpBar);
        hpBar.gameObject.SetActive(false);
    }
}
