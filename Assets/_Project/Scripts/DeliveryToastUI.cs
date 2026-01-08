using System.Collections;
using TMPro;
using UnityEngine;

public class DeliveryToastUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text text;

    [Header("Timings (seconds)")]
    [SerializeField] private float fadeIn = 0.12f;
    [SerializeField] private float hold = 0.55f;
    [SerializeField] private float fadeOut = 0.18f;

    [Header("Motion / Scale")]
    [SerializeField] private float startScale = 0.92f;
    [SerializeField] private float endScale = 1.00f;
    [SerializeField] private float startYOffset = 10f;
    [SerializeField] private float endYOffset = 0f;

    private Coroutine _routine;
    private Vector2 _basePos;

    private void Awake()
    {
        if (panel == null) panel = GetComponent<RectTransform>();
        if (group == null) group = GetComponent<CanvasGroup>();
        if (text == null) text = GetComponentInChildren<TMP_Text>(true);

        _basePos = panel.anchoredPosition;
        HideImmediate();
    }

    public void HideImmediate()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (group != null) group.alpha = 0f;
        if (panel != null)
        {
            panel.localScale = Vector3.one;
            panel.anchoredPosition = _basePos;
        }

        gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (text != null) text.text = message;

        gameObject.SetActive(true);

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        // стартовые значения
        group.alpha = 0f;
        panel.localScale = Vector3.one * startScale;
        panel.anchoredPosition = _basePos + new Vector2(0f, startYOffset);

        // fade in + pop
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeIn);

            group.alpha = k;
            panel.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, EaseOutCubic(k));
            panel.anchoredPosition = Vector2.Lerp(_basePos + new Vector2(0f, startYOffset), _basePos + new Vector2(0f, endYOffset), EaseOutCubic(k));

            yield return null;
        }

        group.alpha = 1f;
        panel.localScale = Vector3.one * endScale;
        panel.anchoredPosition = _basePos + new Vector2(0f, endYOffset);

        // hold
        float h = 0f;
        while (h < hold)
        {
            h += Time.unscaledDeltaTime;
            yield return null;
        }

        // fade out
        float o = 0f;
        while (o < fadeOut)
        {
            o += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(o / fadeOut);
            group.alpha = 1f - k;
            yield return null;
        }

        group.alpha = 0f;
        gameObject.SetActive(false);
        _routine = null;
    }

    private float EaseOutCubic(float x)
    {
        x = Mathf.Clamp01(x);
        return 1f - Mathf.Pow(1f - x, 3f);
    }
}
