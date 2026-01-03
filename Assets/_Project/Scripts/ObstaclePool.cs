using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    [SerializeField] private ObstacleMover obstaclePrefab;
    [SerializeField] private int prewarmCount = 20;

    private readonly Queue<ObstacleMover> _pool = new();

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var obj = Instantiate(obstaclePrefab, transform);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public ObstacleMover Get()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        var obj = Instantiate(obstaclePrefab, transform);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void Release(ObstacleMover obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        _pool.Enqueue(obj);
    }
}
