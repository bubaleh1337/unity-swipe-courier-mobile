using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f; // должно совпадать с LaneMover
    [SerializeField] private int laneCount = 3;

    [Header("Spawn")]
    [SerializeField] private float spawnZ = 18f;            // где появляются препятствия
    [SerializeField] private float spawnInterval = 1.0f;    // раз в N секунд
    [SerializeField] private float startDelay = 1.0f;

    [Header("Movement")]
    [SerializeField] private float obstacleSpeed = 10f;

    private float _timer;

    private void Start()
    {
        _timer = -startDelay;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        int laneIndex = Random.Range(0, laneCount); // 0..2
        int centerIndex = (laneCount - 1) / 2;      // для 3 полос = 1
        float x = (laneIndex - centerIndex) * laneOffset;

        Vector3 pos = new Vector3(x, 0.5f, spawnZ);

        GameObject go = Instantiate(obstaclePrefab, pos, Quaternion.identity);

        // скорость препятствия
        var mover = go.GetComponent<ObstacleMover>();
        if (mover == null) mover = go.AddComponent<ObstacleMover>();
        mover.speed = obstacleSpeed;
    }
}
