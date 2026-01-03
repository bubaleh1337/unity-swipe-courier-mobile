using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 10f;       // будет задаваться спавнером
    public float destroyZ = -8f;    // когда уедет за игрока, уничтожаем

    private void Update()
    {
        transform.position += Vector3.back * (speed * Time.deltaTime);

        if (transform.position.z < destroyZ)
            Destroy(gameObject);
    }
}
