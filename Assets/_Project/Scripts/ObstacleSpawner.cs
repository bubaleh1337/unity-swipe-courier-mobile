using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private ObstaclePool pool;

    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f;
    [SerializeField] private int laneCount = 3;

    [Header("Spawn")]
    [SerializeField] private float spawnZ = 18f;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float startDelay = 1.0f;

    [Header("Movement")]
    [SerializeField] private float obstacleSpeed = 10f;

    [Header("Difficulty")]
    [SerializeField] private float minSpawnInterval = 0.55f;
    [SerializeField] private float maxObstacleSpeed = 16f;
    [SerializeField] private float difficultyRamp = 0.03f;

    private float _timer;

    // fair spawn
    private int _lastLane = -1;
    private int _sameLaneStreak = 0;

    private void Start()
    {
        _timer = -startDelay;

        if (pool == null)
            pool = FindObjectOfType<ObstaclePool>();
    }

    private void Update()
    {
        if (pool == null) return; // <- теперь проверяем pool

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
        // 1) выбрать полосу "честно"
        int laneIndex = Random.Range(0, laneCount);

        if (laneIndex == _lastLane)
        {
            _sameLaneStreak++;
            if (_sameLaneStreak >= 3)
            {
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
        int centerIndex = (laneCount - 1) / 2; // для 3 полос = 1
        float x = (laneIndex - centerIndex) * laneOffset;

        // 3) взять объект из пула
        Vector3 pos = new Vector3(x, 0.5f, spawnZ);
        var mover = pool.Get();
        mover.transform.position = pos;
        mover.transform.rotation = Quaternion.identity;

        // 4) инициализировать скорость и привязку к пулу
        mover.Init(pool, obstacleSpeed);
    }
}
