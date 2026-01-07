using UnityEngine;

public class PackageSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PackagePool pool;
    [SerializeField] private GameManager gm;

    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f; // must match LaneMover
    [SerializeField] private int laneCount = 3;

    [Header("Spawn")]
    [SerializeField] private float spawnZ = 18f;
    [SerializeField] private float spawnY = 0.6f;   // package height above road
    [SerializeField] private float startDelay = 1.0f;

    [SerializeField] private float spawnInterval = 1.4f;
    [SerializeField] private float intervalJitter = 0.6f; // random +/- jitter
    [Range(0f, 1f)]
    [SerializeField] private float spawnChance = 0.55f;   // chance per attempt

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f; // set close to obstacle speed
    [SerializeField] private float despawnZ = -8f;

    private float _timer;
    private float _nextInterval;

    private int _lastLane = -1;
    private int _sameLaneStreak = 0;

    private void Start()
    {
        if (gm == null) gm = FindObjectOfType<GameManager>();
        _timer = -startDelay;
        RollNextInterval();
    }

    private void Update()
    {
        if (gm != null && !gm.IsRunning) return;
        if (Time.timeScale == 0f) return; // paused or result screen

        _timer += Time.deltaTime;
        if (_timer >= _nextInterval)
        {
            _timer = 0f;
            RollNextInterval();
            TrySpawn();
        }
    }

    private void RollNextInterval()
    {
        float j = Random.Range(-intervalJitter, intervalJitter);
        _nextInterval = Mathf.Max(0.3f, spawnInterval + j);
    }

    private void TrySpawn()
    {
        if (Random.value > spawnChance) return;

        GameObject go = pool != null ? pool.Get() : null;
        if (go == null) return;

        int laneIndex = PickLane();

        float x = LaneIndexToX(laneIndex);
        go.transform.position = new Vector3(x, spawnY, spawnZ);
        go.transform.rotation = Quaternion.identity;

        // Init pickup (so it returns to pool and adds packages)
        PackagePickup pickup = go.GetComponent<PackagePickup>();
        if (pickup != null)
            pickup.Init(pool, gm);

        // Move: we keep it simple – move via Rigidbody each frame
        PackageMover mover = go.GetComponent<PackageMover>();
        if (mover != null)
            mover.Init(pool, gm, moveSpeed, despawnZ);
    }

    private int PickLane()
    {
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
        return laneIndex;
    }

    private float LaneIndexToX(int laneIndex)
    {
        float center = (laneCount - 1) * 0.5f;
        return (laneIndex - center) * laneOffset;
    }
}
