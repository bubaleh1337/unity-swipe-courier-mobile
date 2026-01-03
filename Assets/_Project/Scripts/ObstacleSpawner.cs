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

    [Header("Difficulty")]
    [SerializeField] private float minSpawnInterval = 0.55f;
    [SerializeField] private float maxObstacleSpeed = 16f;
    [SerializeField] private float difficultyRamp = 0.03f; // скорость роста сложности

    private float _timer;

    // fair spawn
    private int _lastLane = -1;
    private int _sameLaneStreak = 0;

    private void Start()
    {
        _timer = -startDelay;
    }

    private void Update()
    {
        // Плавно усложняем игру со временем
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - Time.deltaTime * difficultyRamp);
        obstacleSpeed = Mathf.Min(maxObstacleSpeed, obstacleSpeed + Time.deltaTime * (difficultyRamp * 10f));

        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        if (obstaclePrefab == null) return;

        // 1) выбрать полосу "честно"
        int laneIndex = Random.Range(0, laneCount);

        if (laneIndex == _lastLane)
        {
            _sameLaneStreak++;
            if (_sameLaneStreak >= 3)
            {
                // насильно выберем другую полосу
                laneIndex = (laneIndex + Random.Range(1, laneCount)) % laneCount;
                _sameLaneStreak = 0;
            }
        }
        else
        {
            _sameLaneStreak = 0;
        }

        _lastLane = laneIndex;

        // 2) посчитать X по полосам
        int centerIndex = (laneCount - 1) / 2;          // для 3 полос = 1
        float x = (laneIndex - centerIndex) * laneOffset;

        // 3) создать препятствие
        Vector3 pos = new Vector3(x, 0.5f, spawnZ);
        GameObject go = Instantiate(obstaclePrefab, pos, Quaternion.identity);

        // 4) задать скорость движения препятствия
        var mover = go.GetComponent<ObstacleMover>();
        if (mover == null) mover = go.AddComponent<ObstacleMover>();
        mover.speed = obstacleSpeed;
    }
}
