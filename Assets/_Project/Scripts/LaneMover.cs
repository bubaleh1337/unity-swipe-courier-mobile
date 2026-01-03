using UnityEngine;

public class LaneMover : MonoBehaviour
{
    [Header("Lane Setup")]
    [SerializeField] private float laneOffset = 2f; // расстояние между полосами
    [SerializeField] private int laneCount = 3;     // 3 полосы

    [Header("Movement")]
    [SerializeField] private float laneChangeSpeed = 12f;

    private int _currentLane = 1; // 0..2, старт по центру = 1
    private float _targetX;

    private void Start()
    {
        UpdateTarget();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        //if (Input.GetKeyDown(KeyCode.D)) MoveRight();

        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, _targetX, Time.deltaTime * laneChangeSpeed);
        transform.position = pos;
    }

    public void MoveLeft()
    {
        if (_currentLane <= 0) return;
        _currentLane--;
        UpdateTarget();
    }

    public void MoveRight()
    {
        if (_currentLane >= laneCount - 1) return;
        _currentLane++;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        // центрируем полосы вокруг 0: при 3 полосах индексы 0,1,2 → x = -offset,0,+offset
        int centerIndex = (laneCount - 1) / 2;
        _targetX = (_currentLane - centerIndex) * laneOffset;
    }
}
