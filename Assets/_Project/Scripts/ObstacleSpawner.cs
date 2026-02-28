using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private ObstaclePool pool;

    [Header("Road (for perfect centering)")]
    [SerializeField] private BoxCollider roadCollider;
    [SerializeField] private float edgePadding = 0.6f; // отступ от бортиков

    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f;   // можно оставить, если roadCollider не задан
    [SerializeField] private int laneCount = 3;


    [Header("Spawn")]
    [SerializeField] private float spawnZ = 18f;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float startDelay = 1.0f;

    [Header("Movement")]
    [SerializeField] private float obstacleSpeed = 10f;

    [Header("Difficulty")]
    [SerializeField] private DifficultyController difficulty;


    private float _timer;
    private GameManager _gm;


    // fair spawn
    private int _lastLane = -1;
    private int _sameLaneStreak = 0;
    public float CurrentSpeed => obstacleSpeed;

    private void Start()
    {
        _timer = -startDelay;
        _gm = FindObjectOfType<GameManager>();

        if (difficulty == null) difficulty = FindObjectOfType<DifficultyController>();

        if (pool == null)
            pool = FindObjectOfType<ObstaclePool>();
    }

    private void Update()
    {
        if (pool == null) return;
        if (_gm != null && !_gm.IsRunning) return;

        // Плавно усложняем игру со временем
        if (difficulty != null)
        {
            spawnInterval = difficulty.SpawnInterval;
            obstacleSpeed = difficulty.ObstacleSpeed;
        }

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

        // 2) взять объект из пула
        var mover = pool.Get();
        if (mover == null) return;

        // 3) посчитать X/Y
        float x;
        float y;

        if (roadCollider != null)
        {
            var b = roadCollider.bounds;
            float left = b.min.x + edgePadding;
            float right = b.max.x - edgePadding;

            float laneWidth = (right - left) / laneCount;
            x = left + laneWidth * (laneIndex + 0.5f);

            // Y = верх дороги + половина высоты препятствия
            y = b.max.y;
            var col = mover.GetComponentInChildren<Collider>();
            if (col != null) y += col.bounds.extents.y;
        }
        else
        {
            // fallback (как раньше)
            int centerIndex = (laneCount - 1) / 2;
            x = (laneIndex - centerIndex) * laneOffset;
            y = 0.5f;
        }

        // 4) поставить и запустить 
        mover.transform.position = new Vector3(x, y, spawnZ);
        mover.transform.rotation = Quaternion.identity;
        mover.Init(pool, obstacleSpeed);
    }
}
