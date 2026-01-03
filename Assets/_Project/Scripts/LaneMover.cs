using UnityEngine;

public class LaneMover : MonoBehaviour
{
    [Header("Lane Setup")]
    [SerializeField] private float laneOffset = 2f;
    [SerializeField] private int laneCount = 3;

    [Header("Movement")]
    [SerializeField] private float laneChangeDuration = 0.12f; // сек, плавность
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Visual Tilt")]
    [SerializeField] private float tiltAngle = 12f;     // градусов
    [SerializeField] private float tiltReturnSpeed = 16f; // скорость возврата

    private int _lane;          // 0..laneCount-1
    private float _fromX;
    private float _toX;
    private float _t;
    private bool _moving;

    private Quaternion _baseRot;
    private float _tiltTarget;

    private void Awake()
    {
        _baseRot = transform.rotation;

        // Стартуем в средней полосе
        _lane = Mathf.Clamp(laneCount / 2, 0, laneCount - 1);

        float x = LaneToX(_lane);
        Vector3 p = transform.position;
        transform.position = new Vector3(x, p.y, p.z);

        _fromX = x;
        _toX = x;
        _t = 1f;
        _moving = false;
        _tiltTarget = 0f;
    }

    private void Update()
    {
        // Плавное перемещение по X
        if (_moving)
        {
            _t += Time.deltaTime / Mathf.Max(0.0001f, laneChangeDuration);
            float k = ease.Evaluate(Mathf.Clamp01(_t));
            float x = Mathf.Lerp(_fromX, _toX, k);

            Vector3 p = transform.position;
            transform.position = new Vector3(x, p.y, p.z);

            if (_t >= 1f)
                _moving = false;
        }

        // Визуальный наклон
        float currentZ = Mathf.LerpAngle(0f, _tiltTarget, Time.deltaTime * tiltReturnSpeed);
        // Чтобы наклон был “живой”, мы не ставим сразу baseRot,
        // а аккуратно возвращаемся к целевой величине
        transform.rotation = _baseRot * Quaternion.Euler(0f, 0f, currentZ);

        // После рывка всегда плавно возвращаем наклон к 0
        _tiltTarget = Mathf.Lerp(_tiltTarget, 0f, Time.deltaTime * tiltReturnSpeed);
    }

    public void MoveLeft()
    {
        SetLane(_lane - 1);
        _tiltTarget = +tiltAngle; // влево наклон вправо-влево можно на вкус
    }

    public void MoveRight()
    {
        SetLane(_lane + 1);
        _tiltTarget = -tiltAngle;
    }

    private void SetLane(int newLane)
    {
        newLane = Mathf.Clamp(newLane, 0, laneCount - 1);
        if (newLane == _lane) return;

        _lane = newLane;

        _fromX = transform.position.x;
        _toX = LaneToX(_lane);
        _t = 0f;
        _moving = true;
    }

    private float LaneToX(int laneIndex)
    {
        // laneCount=3 => индексы 0,1,2 => x = -2,0,+2 при offset=2
        float center = (laneCount - 1) * 0.5f;
        return (laneIndex - center) * laneOffset;
    }
}
