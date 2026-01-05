using System.Collections.Generic;
using UnityEngine;

public class PackagePool : MonoBehaviour
{
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private int prewarm = 8;
    [SerializeField] private int maxSize = 16;

    private readonly Queue<GameObject> _inactive = new();
    private int _created;

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < prewarm; i++)
            CreateAndStore();
    }

    private GameObject CreateAndStore()
    {
        GameObject go = Instantiate(packagePrefab, transform);
        go.name = packagePrefab.name + "(Pooled)";
        go.SetActive(false);
        _inactive.Enqueue(go);
        _created++;
        return go;
    }

    public GameObject Get()
    {
        if (_inactive.Count > 0)
        {
            GameObject go = _inactive.Dequeue();
            go.SetActive(true);
            return go;
        }

        if (_created < maxSize)
        {
            GameObject go = CreateAndStore();
            go.SetActive(true);
            return go;
        }

        // Pool is exhausted – skip spawn
        return null;
    }

    public void Return(GameObject go)
    {
        if (go == null) return;

        go.SetActive(false);
        go.transform.SetParent(transform);
        _inactive.Enqueue(go);
    }
}
