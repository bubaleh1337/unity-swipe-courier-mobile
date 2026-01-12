using System.Collections;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    public static VfxPool Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem pickupPrefab;
    [SerializeField] private ParticleSystem deliveryPrefab;

    [Header("Prewarm")]
    [SerializeField] private int pickupPrewarm = 10;
    [SerializeField] private int deliveryPrewarm = 5;

    private ParticleSystem[] _pickupPool;
    private ParticleSystem[] _deliveryPool;
    private int _pickupIndex;
    private int _deliveryIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _pickupPool = new ParticleSystem[Mathf.Max(1, pickupPrewarm)];
        _deliveryPool = new ParticleSystem[Mathf.Max(1, deliveryPrewarm)];

        for (int i = 0; i < _pickupPool.Length; i++)
            _pickupPool[i] = CreateInstance(pickupPrefab);

        for (int i = 0; i < _deliveryPool.Length; i++)
            _deliveryPool[i] = CreateInstance(deliveryPrefab);
    }

    public void SpawnPickup(Vector3 worldPos)
    {
        var ps = GetFromPool(_pickupPool, ref _pickupIndex);
        PlayAt(ps, worldPos);
    }

    public void SpawnDelivery(Vector3 worldPos)
    {
        var ps = GetFromPool(_deliveryPool, ref _deliveryIndex);
        PlayAt(ps, worldPos);
    }

    private ParticleSystem CreateInstance(ParticleSystem prefab)
    {
        if (prefab == null) return null;

        var inst = Instantiate(prefab, transform);
        inst.gameObject.SetActive(false);
        return inst;
    }

    private ParticleSystem GetFromPool(ParticleSystem[] pool, ref int index)
    {
        if (pool == null || pool.Length == 0) return null;

        var ps = pool[index];
        index = (index + 1) % pool.Length;
        return ps;
    }

    private void PlayAt(ParticleSystem ps, Vector3 worldPos)
    {
        if (ps == null) return;

        ps.transform.position = worldPos;
        ps.gameObject.SetActive(true);

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);

        StartCoroutine(DisableAfter(ps));
    }

    private IEnumerator DisableAfter(ParticleSystem ps)
    {
        if (ps == null) yield break;

        // ждём по unscaled времени, чтобы работало даже при Pause
        float t = 0f;
        float wait = Mathf.Max(0.2f, ps.main.duration + ps.main.startLifetime.constantMax);

        while (t < wait)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (ps != null)
            ps.gameObject.SetActive(false);
    }
}
