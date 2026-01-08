using System.Collections;
using UnityEngine;

public class PlayerPopFeedback : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Pop")]
    [SerializeField] private float duration = 0.10f;
    [SerializeField] private float scaleUp = 1.08f;

    private Coroutine _routine;
    private Vector3 _baseScale;

    private void Awake()
    {
        if (target == null) target = transform;
        _baseScale = target.localScale;
    }

    public void Pop()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        float half = Mathf.Max(0.01f, duration * 0.5f);

        // up
        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            target.localScale = Vector3.Lerp(_baseScale, _baseScale * scaleUp, k);
            yield return null;
        }

        // down
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / half);
            target.localScale = Vector3.Lerp(_baseScale * scaleUp, _baseScale, k);
            yield return null;
        }

        target.localScale = _baseScale;
        _routine = null;
    }
}
