using System.Collections;
using UnityEngine;

public class PlayerPopFeedback : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;   // что "попаем" (например, модель игрока или весь Player)

    [Header("Pop")]
    [SerializeField] private float popScale = 1.07f;
    [SerializeField] private float popIn = 0.06f;
    [SerializeField] private float popOut = 0.10f;

    private Vector3 _baseScale;
    private Coroutine _routine;

    private void Awake()
    {
        if (target == null) target = transform;
        _baseScale = target.localScale;
    }

    public void Play()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        // In
        float t = 0f;
        Vector3 from = _baseScale;
        Vector3 to = _baseScale * popScale;

        while (t < popIn)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / popIn);
            target.localScale = Vector3.Lerp(from, to, EaseOut(k));
            yield return null;
        }
        target.localScale = to;

        // Out
        t = 0f;
        while (t < popOut)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / popOut);
            target.localScale = Vector3.Lerp(to, _baseScale, EaseOut(k));
            yield return null;
        }

        target.localScale = _baseScale;
        _routine = null;
    }

    private static float EaseOut(float x)
    {
        // квадратичная easeOut
        return 1f - (1f - x) * (1f - x);
    }
}
