using System.Collections;
using TMPro;
using UnityEngine;

public class DeliveryToastUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;   // RectTransform на DeliveryToast
    [SerializeField] private TMP_Text label;        // TMP Text внутри

    [Header("Timing (unscaled)")]
    [SerializeField] private float enterDuration = 0.12f;
    [SerializeField] private float stayDuration = 0.90f;
    [SerializeField] private float exitDuration = 0.18f;

    [Header("Motion")]
    [SerializeField] private Vector2 hiddenOffset = new Vector2(0f, 40f);
    [SerializeField] private float startScale = 0.96f;

    private Coroutine _routine;
    private Vector2 _shownPos;
    private Vector2 _hiddenPos;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (panel == null) panel = GetComponent<RectTransform>();

        _shownPos = panel.anchoredPosition;
        _hiddenPos = _shownPos + hiddenOffset;

        HideImmediate();
    }

    public void HideImmediate()
    {
        ApplyState(0f);
    }

    public void Show(string message)
    {
        if (label != null)
            label.text = message;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // ENTER
        float t = 0f;
        while (t < enterDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / enterDuration);
            ApplyState(EaseOutCubic(k));
            yield return null;
        }
        ApplyState(1f);

        // STAY
        float s = 0f;
        while (s < stayDuration)
        {
            s += Time.unscaledDeltaTime;
            yield return null;
        }

        // EXIT
        t = 0f;
        while (t < exitDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / exitDuration);
            ApplyState(1f - EaseInCubic(k));
            yield return null;
        }
        ApplyState(0f);

        _routine = null;
    }

    private void ApplyState(float a)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = a;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (panel != null)
        {
            panel.anchoredPosition = Vector2.Lerp(_hiddenPos, _shownPos, a);

            float sc = Mathf.Lerp(startScale, 1f, a);
            panel.localScale = new Vector3(sc, sc, sc);
        }
    }

    private static float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    private static float EaseInCubic(float x)
    {
        return x * x * x;
    }
}
