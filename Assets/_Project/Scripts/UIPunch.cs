using System.Collections;
using UnityEngine;

public class UIPunch : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float upScale = 1.08f;
    [SerializeField] private float upTime = 0.06f;
    [SerializeField] private float downTime = 0.10f;

    private Coroutine _routine;
    private Vector3 _baseScale;

    private void Awake()
    {
        if (target == null) target = GetComponent<RectTransform>();
        if (target != null) _baseScale = target.localScale;
    }

    public void Punch()
    {
        if (target == null) return;

        // базовый скейл фиксируем на момент удара
        _baseScale = target.localScale;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        // UP
        float t = 0f;
        while (t < upTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / upTime);
            float sc = Mathf.Lerp(1f, upScale, EaseOutQuad(k));
            target.localScale = _baseScale * sc;
            yield return null;
        }

        // DOWN
        t = 0f;
        while (t < downTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / downTime);
            float sc = Mathf.Lerp(upScale, 1f, EaseInQuad(k));
            target.localScale = _baseScale * sc;
            yield return null;
        }

        target.localScale = _baseScale;
        _routine = null;
    }

    private static float EaseOutQuad(float x) => 1f - (1f - x) * (1f - x);
    private static float EaseInQuad(float x) => x * x;
}
