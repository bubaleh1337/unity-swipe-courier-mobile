using UnityEngine;

public class CourierBoxStack : MonoBehaviour
{
    [Header("Box visuals (0..3)")]
    [SerializeField] private GameObject box1;
    [SerializeField] private GameObject box2;
    [SerializeField] private GameObject box3;

    public void SetCount(int count)
    {
        count = Mathf.Clamp(count, 0, 3);

        if (box1 != null) box1.SetActive(count >= 1);
        if (box2 != null) box2.SetActive(count >= 2);
        if (box3 != null) box3.SetActive(count >= 3);
    }

    public void Clear()
    {
        SetCount(0);
    }
}
