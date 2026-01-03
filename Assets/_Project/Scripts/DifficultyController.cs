using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [Header("Difficulty")]
    [SerializeField] private float rampPerSecond = 0.03f;   // скорость роста сложности
    [SerializeField] private float maxDifficulty = 1.0f;    // 0..1

    [Header("Spawn Interval")]
    [SerializeField] private float startSpawnInterval = 1.0f;
    [SerializeField] private float minSpawnInterval = 0.55f;

    [Header("Obstacle Speed")]
    [SerializeField] private float startObstacleSpeed = 10f;
    [SerializeField] private float maxObstacleSpeed = 16f;

    [Header("Score")]
    [SerializeField] private float startScorePerSecond = 5f;
    [SerializeField] private float maxScorePerSecond = 9f;

    public float Difficulty01 { get; private set; } // 0..1
    public float SpawnInterval { get; private set; }
    public float ObstacleSpeed { get; private set; }
    public float ScorePerSecond { get; private set; }

    private GameManager _gm;

    private void Awake()
    {
        _gm = FindObjectOfType<GameManager>();
        ResetValues();
    }

    private void Update()
    {
        if (_gm != null && (!_gm.IsRunning || _gm.IsPaused)) return;

        Difficulty01 = Mathf.Clamp01(Difficulty01 + rampPerSecond * Time.deltaTime);

        SpawnInterval = Mathf.Lerp(startSpawnInterval, minSpawnInterval, Difficulty01);
        ObstacleSpeed = Mathf.Lerp(startObstacleSpeed, maxObstacleSpeed, Difficulty01);
        ScorePerSecond = Mathf.Lerp(startScorePerSecond, maxScorePerSecond, Difficulty01);
    }

    public void ResetValues()
    {
        Difficulty01 = 0f;
        SpawnInterval = startSpawnInterval;
        ObstacleSpeed = startObstacleSpeed;
        ScorePerSecond = startScorePerSecond;
    }
}
