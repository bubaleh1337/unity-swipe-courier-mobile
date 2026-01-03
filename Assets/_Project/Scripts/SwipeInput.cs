using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    [SerializeField] private LaneMover laneMover;
    [SerializeField] private float minSwipeDistance = 80f; // пиксели

    private Vector2 _startPos;
    private bool _isSwiping;

    private void Awake()
    {
        if (laneMover == null)
            laneMover = FindObjectOfType<LaneMover>();
    }

    private void Update()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm != null && !gm.IsRunning) return;

        // Поддержка и мыши (для теста в Editor), и тача (на телефоне)
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
            _isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && _isSwiping)
        {
            Vector2 endPos = Input.mousePosition;
            HandleSwipe(_startPos, endPos);
            _isSwiping = false;
        }

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                _startPos = t.position;
                _isSwiping = true;
            }
            else if ((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) && _isSwiping)
            {
                HandleSwipe(_startPos, t.position);
                _isSwiping = false;
            }
        }
    }

    private void HandleSwipe(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;

        if (Mathf.Abs(delta.x) < minSwipeDistance) return;
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;


        if (delta.x > 0) laneMover.MoveRight();
        else laneMover.MoveLeft();
    }
}
