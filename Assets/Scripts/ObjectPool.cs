using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance<T>() where T : MonoBehaviour
    {
        ObjectPool[] pools = FindObjectsOfType<ObjectPool>();
        foreach (ObjectPool pool in pools)
        {
            if (pool.objPrefab.TryGetComponent<T>(out _))
            {
                return pool;
            }
        }
        return null;
    }

    public GameObject objPrefab;
    const int poolSize = 10;

    private Stack<GameObject> availableObjects;

    public void Init()
    {
        availableObjects = new Stack<GameObject>();
        SpawnPool();
    }

    private void SpawnPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objPrefab, transform);
            obj.SetActive(false);
            availableObjects.Push(obj);
        }
    }

    public GameObject GetObject()
    {
        if (availableObjects.Count == 0)
        {
            SpawnPool();
        }
        GameObject obj = availableObjects.Pop();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        availableObjects.Push(obj);
        obj.SetActive(false);
    }
}
