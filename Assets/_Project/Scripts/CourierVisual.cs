using UnityEngine;

public class CourierVisual : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject carriedBox; // объект на Backpack

    [Header("Behavior")]
    [SerializeField] private int visibleBoxesMax = 1;

    private int _visibleCount;

    private void Awake()
    {
        SetVisible(false);
    }

    public void ResetVisual()
    {
        _visibleCount = 0;
        SetVisible(false);
    }

    public void OnPackagePicked()
    {
        _visibleCount = Mathf.Clamp(_visibleCount + 1, 0, visibleBoxesMax);
        SetVisible(_visibleCount > 0);
    }

    public void OnDeliveryComplete()
    {
        _visibleCount = 0;
        SetVisible(false);
    }

    private void SetVisible(bool value)
    {
        if (carriedBox != null)
            carriedBox.SetActive(value);
    }
}
