using UnityEngine;

public class LevelLooper : MonoBehaviour
{
    [SerializeField] private Transform[] segments; // Segment_00..02
    [SerializeField] private float segmentLength = 30f;
    [SerializeField] private float speed = 10f;

    private void Update()
    {
        // двигаем каждый сегмент к камере (назад по Z)
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].Translate(0f, 0f, -speed * Time.deltaTime, Space.World);

            // если сегмент полностью ушЄл назад Ч переносим вперЄд
            if (segments[i].position.z <= -segmentLength)
            {
                float forward = segmentLength * segments.Length;
                segments[i].position += new Vector3(0f, 0f, forward);
            }
        }
    }
}
