using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool:MonoBehaviour
{
    public GameObject hpBarPrefab;
    const int poolSize=30;

    private static Queue<HpBar> availableHpBar;

    private void Start()
    {
        availableHpBar = new Queue<HpBar>();
        for(int i = 0; i < poolSize; i++)
        {
            HpBar hpBar = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
            hpBar.gameObject.SetActive(false);
            availableHpBar.Enqueue(hpBar);
        }
    }

    public static HpBar GetHpBar(Character character)
    {
        HpBar hpBar = availableHpBar.Dequeue();
        hpBar.gameObject.SetActive(true);
        hpBar.Init(character);
        return hpBar;
    }

    public static void ReturnHpBar(HpBar hpBar)
    {
        availableHpBar.Enqueue(hpBar);
        hpBar.gameObject.SetActive(false);
    }
}
