using System.Collections;
using TMPro;
using UnityEngine;

public class DeliveryToastAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform toastRoot;   // обычно это RectTransform самого DeliveryToast
    [SerializeField] private CanvasGroup canvasGroup;   // CanvasGroup на DeliveryToast
    [SerializeField] private TMP_Text messageText;      // DeliveryToast/Text (TMP)

    [Header("Animation")]
    [SerializeField] private float enterDuration = 0.18f;
    [SerializeField] private float stayDuration = 0.85f;
    [SerializeField] private float exitDuration = 0.22f;

    [SerializeField] private float offsetY = 26f;       // смещение для въезда/выезда
    [SerializeField] private float startScale = 0.96f;  // лёгкий "pop" при появлении

    private Vector2 _baseAnchoredPos;
    private Coroutine _routine;

    private void Awake()
    {
        if (toastRoot == null) toastRoot = GetComponent<RectTransform>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        _baseAnchoredPos = toastRoot.anchoredPosition;

        // стартовое состояние
        canvasGroup.alpha = 0f;
        toastRoot.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (messageText != null)
            messageText.text = message;

        if (_routine != null)
            StopCoroutine(_routine);

        gameObject.SetActive(true);
        _routine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // Enter
        float t = 0f;
        canvasGroup.alpha = 0f;
        toastRoot.anchoredPosition = _baseAnchoredPos + new Vector2(0f, offsetY);
        toastRoot.localScale = Vector3.one * startScale;

        while (t < enterDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / enterDuration);

            // smooth
            float s = Smooth01(k);

            canvasGroup.alpha = s;
            toastRoot.anchoredPosition = Vector2.Lerp(
                _baseAnchoredPos + new Vector2(0f, offsetY),
                _baseAnchoredPos,
                s
            );
            toastRoot.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one, s);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        toastRoot.anchoredPosition = _baseAnchoredPos;
        toastRoot.localScale = Vector3.one;

        // Stay
        float stay = 0f;
        while (stay < stayDuration)
        {
            stay += Time.unscaledDeltaTime;
            yield return null;
        }

        // Exit
        t = 0f;
        while (t < exitDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / exitDuration);
            float s = Smooth01(k);

            canvasGroup.alpha = 1f - s;
            toastRoot.anchoredPosition = Vector2.Lerp(
                _baseAnchoredPos,
                _baseAnchoredPos + new Vector2(0f, offsetY),
                s
            );

            yield return null;
        }

        canvasGroup.alpha = 0f;
        toastRoot.anchoredPosition = _baseAnchoredPos;
        toastRoot.localScale = Vector3.one;

        _routine = null;
        gameObject.SetActive(false);
    }

    private static float Smooth01(float x)
    {
        // Smoothstep
        return x * x * (3f - 2f * x);
    }
}
